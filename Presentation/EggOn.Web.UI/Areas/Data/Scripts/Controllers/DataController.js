angular.module('EggOn.Data')

    .controller('DataController', ['$scope', 'Containers', function ($scope, Containers) {
        console.log('data controller');

        $scope.changePageTitle('Data');

        $scope.loadContainers = function () {
            $scope.showLoadingMessage = true;
            $scope.showErrorMessage = false;
            $scope.containers = Containers.query(function () {
                $scope.showLoadingMessage = false;
            }, function () {
                $scope.showLoadingMessage = false;
                $scope.showErrorMessage = true;
            });
        };
        $scope.loadContainers();

        $scope.$on('container::added', function (e, container) {
            $scope.containers.push(container);
        });
        $scope.$on('container::updated', function (e, container) {
            $scope.containers.replace(container, function (c) { return container.Id == c.Id; });
        });
        $scope.$on('container::removed', function (e, container) {
            $scope.containers.remove(function (c) { return container.Id == c.Id; });
        });
    } ]);