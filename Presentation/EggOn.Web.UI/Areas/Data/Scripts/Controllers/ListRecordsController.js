angular.module('EggOn.Data')

    .controller('ListRecordsController', ['$scope', '$state', '$stateParams', '$modal', 'Containers', 'Records',
    function ($scope, $state, $stateParams, $modal, Containers, Records) {
        console.log('data details controller');

        $scope.loading = true;

        $scope.records = [];
        $scope.container = Containers.get({ containerId: $stateParams.containerId }, function (container) {
            $scope.changePageTitle(container.Name + ' - Data');

            $scope.records = Records.query({ containerId: container.Id }, function () {
                $scope.loading = false;
            });
        });

        $scope.getTypeName = function (field) {
            if (!$scope.container)
                return null;

            var type = $scope.container.Fields.find(function (f) { return f.Id == field.FieldTypeId; });
            return (type) ? type.Name : null;
        };

        $scope.addRecord = function () {
            $state.transitionTo('manager.data.details.createRecord', { containerId: $scope.container.Id }, true);
        };

        $scope.editRecord = function (record) {
            var primaryField = $scope.container.Fields.find(function (f) { return f.PrimaryKey });

            $state.transitionTo('manager.data.details.editRecord', { containerId: $scope.container.Id, recordId: record[primaryField.ColumnName] }, true);
        };

        $scope.deleteRecord = function (record) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Record', body: 'Are you sure you want to delete this record?' }; } }
            }).result.then(function () {
                var primaryField = $scope.container.Fields.find(function(f) { return f.PrimaryKey });
                Records.remove({ containerId: $scope.container.Id, recordId: record[primaryField.ColumnName] }, function (deletedRecord) {
                    $scope.$emit('record::removed', deletedRecord);
                    $scope.records.splice($scope.records.indexOf(record), 1);
                });
            });
        };

        $scope.editContainer = function () {
            $state.transitionTo('manager.data.details.edit', { containerId: $scope.container.Id }, true);
        };

        $scope.deleteContainer = function () {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Container', body: 'Are you sure you want to delete this container?' }; } }
            }).result.then(function () {
                Containers.remove({ containerId: $scope.container.Id }, function (container) {
                    $scope.$emit('container::removed', container);
                    $state.transitionTo('manager.data.dashboard', {}, true);
                });
            });
        };

        $scope.isImage = function (eggonFilePath) {
            return eggonFilePath && (
                   eggonFilePath.indexOf('.bmp') != -1 ||
                   eggonFilePath.indexOf('.png') != -1 ||
                   eggonFilePath.indexOf('.jpeg') != -1 ||
                   eggonFilePath.indexOf('.jpg') != -1);
        };
    } ]);