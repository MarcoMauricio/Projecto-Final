angular.module('EggOn.Files')

    .controller('FilesViewController', ['$scope', '$state', '$stateParams', '$modal', 'Repositories',
    function ($scope, $state, $stateParams, $modal, Repositories) {

        $scope.changePageTitle('Files');

        $scope.repository = Repositories.get({ repositoryId: $stateParams.repositoryId }, function (repository) {
            $scope.changePageTitle(repository.Name + ' - Files');
        });

        $scope.selectedFile = null;

        $scope.deleteRepository = function () {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Repository', body: 'Are you sure you want to delete this repository?' }; } }
            }).result.then(function () {
                Repositories.remove({ repositoryId: $scope.repository.Id }, function (repository) {
                    $scope.$emit('repository::removed', repository);
                    $state.transitionTo('manager.files.dashboard', {}, true);
                });
            });
        };
    } ]);