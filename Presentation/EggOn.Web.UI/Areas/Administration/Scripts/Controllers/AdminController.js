angular.module('EggOn.Administration')

    .controller('AdminController', ['$scope', '$state',
    function ($scope, $state) {
        $scope.changePageTitle('Administration');
    }]);