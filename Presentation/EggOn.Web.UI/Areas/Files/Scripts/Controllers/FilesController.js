angular.module('EggOn.Files')

    .controller('FilesController', ['$scope', 'Repositories', function ($scope, Repositories) {
        console.log('files controller');

        $scope.changePageTitle('Files');

        $scope.loadRepositories = function () {
            $scope.showLoadingMessage = true;
            $scope.showErrorMessage = false;
            $scope.repositories = Repositories.query(function () {
                $scope.showLoadingMessage = false;
            }, function () {
                $scope.showLoadingMessage = false;
                $scope.showErrorMessage = true;
            });
        };
        $scope.loadRepositories();

        $scope.$on('repository::added', function (e, repository) {
            $scope.repositories.push(repository);
        });
        $scope.$on('repository::updated', function (e, repository) {
            $scope.repositories.replace(repository, function (r) { return repository.Id == r.Id; });
        });
        $scope.$on('repository::removed', function (e, repository) {
            $scope.repositories.remove(function (r) { return repository.Id == r.Id; });
        });
    }]);