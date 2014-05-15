angular.module('EggOn.Data')

    .controller('ContainerController', ['$scope', '$modal', '$state', '$stateParams', 'Containers', 'FieldTypes',
    function ($scope, $modal, $state, $stateParams, Containers, FieldTypes) {
        $scope.saving = false;
        $scope.saveSuccess = $scope.saveError = false;

        $scope.containerTypes = [{ Id: 1, Name: 'Local' }];
        $scope.fieldTypes = FieldTypes.query();

        if (!$stateParams.containerId) {
            $scope.changePageTitle('Create Container - Data');

            $scope.container = {
                Name: '',
                Type: 1,
                Fields: []
            };

            $scope.primaryField = null;
        } else {
            $scope.changePageTitle('Edit Container - Data');

            $scope.container = Containers.get({ containerId: $stateParams.containerId }, function (container) {
                $scope.primaryField = container.Fields.find(function (f) { return f.PrimaryKey; });
            });
        }

        $scope.typeIdToName = function (id) {
            var type = $scope.fieldTypes.find(function (t) { return t.Id == id; });

            if (type != null)
                return type.Name;

            return '';
        };

        $scope.addField = function () {
            var types = $scope.fieldTypes;
            var fields = $scope.container.Fields;

            $modal.open({
                templateUrl: '/Areas/Data/Views/Modals/Field.html',
                controller: ['$scope', '$modalInstance', function ($scope, $modalInstance) {
                    $scope.fieldTypes = types;
                    $scope.field = {
                        Name: '',
                        FieldTypeId: (types.length != 0) ? types[0].Id : null,
                        Order: 0,
                        ShowInList: false
                    };

                    $scope.cancel = function () {
                        $modalInstance.dismiss('close');
                    };

                    $scope.saveField = function () {
                        fields.push($scope.field);
                        $modalInstance.close('ok');
                    };
                }]
            });
        };

        $scope.editField = function (field) {
            var types = $scope.fieldTypes;
            var fields = $scope.container.Fields;

            $modal.open({
                templateUrl: '/Areas/Data/Views/Modals/Field.html',
                controller: ['$scope', '$modalInstance', function ($scope, $modalInstance) {
                    $scope.fieldTypes = types;
                    $scope.field = angular.copy(field);

                    $scope.cancel = function () {
                        $modalInstance.dismiss('close');
                    };

                    $scope.saveField = function () {
                        angular.copy($scope.field, field);
                        $modalInstance.close('ok');
                    };
                }]
            });
        };

        $scope.removeField = function (field) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Field', body: 'Are you sure you want to delete this field?' }; } }
            }).result.then(function () {
                var index = $scope.container.Fields.indexOf(field);
                $scope.container.Fields.splice(index, 1);
            });
        };

        $scope.changedPrimaryField = function (newPrimaryField) {
            for (var i = 0; i < $scope.container.Fields.length; i++) {
                $scope.container.Fields[i].PrimaryKey = ($scope.container.Fields[i] == newPrimaryField);
            }
        };

        $scope.isPrimaryType = function (field) {
            var fieldType = $scope.fieldTypes.find(function (ft) { return ft.Id == field.FieldTypeId; });
            return fieldType != null && fieldType.CanBePrimary;
        }

        $scope.submitContainer = function () {
            $scope.saving = true;
            $scope.saveSuccess = $scope.saveError = false;

            if (!$stateParams.containerId) {
                Containers.save($scope.container, function (container) {
                    $scope.saving = false;
                    $scope.saveSuccess = true;
                    $scope.container = container;
                    $scope.$emit('container::added', container);
                    $state.transitionTo('manager.data.details.showRecords', { containerId: container.Id }, true);
                }, function () {
                    $scope.saving = false;
                    $scope.saveError = true;
                });
            } else {
                Containers.update($scope.container, function (container) {
                    $scope.saving = false;
                    $scope.saveSuccess = true;
                    $scope.container = container;
                    $scope.$emit('container::updated', container);
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