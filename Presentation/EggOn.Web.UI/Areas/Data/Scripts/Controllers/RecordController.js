angular.module('EggOn.Data')

    .controller('RecordController', ['$scope', '$state', '$stateParams', '$modal', 'Containers', 'FieldTypes', 'Records',
    function ($scope, $state, $stateParams, $modal, Containers, FieldTypes, Records) {
        console.log('data details controller');

        $scope.record = {};
        $scope.fieldTypes = FieldTypes.query();

        $scope.loading = true;

        $scope.container = Containers.get({ containerId: $stateParams.containerId }, function (container) {

            if ($stateParams.recordId == null) {
                $scope.changePageTitle('Create Record - ' + container.Name + ' - Data');

                for (var i = 0; i < container.Fields.length; i++) {
                    $scope.record[container.Fields[i].ColumnName] = container.Fields[i].DefaultValue;
                }

                $scope.loading = false;
            } else {
                $scope.changePageTitle('Edit Record - ' + $scope.container.Name + ' - Data');

                $scope.record = Records.get({ containerId: container.Id, recordId: $stateParams.recordId }, function (record) {                   
                    $scope.loading = false;
                });
            }
        });

        $scope.getTypeName = function (field) {
            var fieldType = $scope.fieldTypes.find(function (f) { return f.Id == field.FieldTypeId; });
            return (fieldType) ? fieldType.Name : null;
        };

        $scope.typeIsPrimary = function (field) {
            if ($scope.fieldTypes.length == 0)
                return;

            var fieldType = $scope.fieldTypes.find(function (f) { return f.Id == field.FieldTypeId; });
            return fieldType != null && fieldType.CanBePrimary;
        };

        $scope.fieldIs = function (field, typeName) {
            if ($scope.fieldTypes.length == 0)
                return;

            var fieldType = $scope.fieldTypes.find(function (t) { return t.Id == field.FieldTypeId; });
            return fieldType != null && fieldType.Name == typeName;
        };

        $scope.saving = false;
        $scope.saveSuccess = $scope.saveError = false;

        $scope.submitRecord = function () {
            $scope.saving = true;
            $scope.saveSuccess = $scope.saveError = false;

            if ($stateParams.recordId == null) {
                Records.save({ containerId: $scope.container.Id }, $scope.record, function (record) {
                    $scope.saving = false;
                    $scope.saveSuccess = true;
                    $state.transitionTo('manager.data.details.showRecords', { containerId: $scope.container.Id }, true);
                    $scope.$emit('record::added', record);
                }, function () {
                    $scope.saving = false;
                    $scope.saveError = true;
                });
            } else {
                var primaryField = $scope.container.Fields.find(function (f) { return f.PrimaryKey });

                Records.update({ containerId: $scope.container.Id, recordId: $scope.record[primaryField.ColumnName] }, $scope.record, function (record) {
                    $scope.saving = false;
                    $scope.saveSuccess = true;
                    $scope.$emit('record::updated', record);
                }, function () {
                    $scope.saving = false;
                    $scope.saveError = true;
                });
            }
        };

        $scope.goBack = function () {
            $state.transitionTo('manager.data.details.showRecords', { containerId: $scope.container.Id }, true);
        };
    } ]);