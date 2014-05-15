angular.module('EggOn.Files')

    .controller('FilesCreateController', ['$scope', '$state', 'Repositories', 
    function ($scope, $state, Repositories) {
        console.log('files create controller');

        $scope.changePageTitle('Create - Files');

        $scope.saving = false;
        $scope.saveSuccess = $scope.saveError = false;

        $scope.repository = { Name: '', Type: 1 };

        $scope.createRepository = function () {
            $scope.saving = true;
            $scope.saveSuccess = $scope.saveError = false;
            Repositories.save($scope.repository, function (repository) {
                $scope.saving = false;
                $scope.saveSuccess = true;
                $scope.$emit('repository::added', repository);
                $state.transitionTo('manager.files.details', { repositoryId: repository.Id }, true);
            }, function () {
                $scope.saving = false;
                $scope.saveError = true;
            });
        };
    } ]);