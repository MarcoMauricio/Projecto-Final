angular.module('EggOn.Administration')

    .controller('AdminLayoutController', ['$scope', '$modal', '$http',
    function ($scope, $modal, $http) {
        $scope.changePageTitle('Layout - Administration');

        $scope.showLoadingMessage = true;
        $http.get(WebServiceUrl + 'config/layout')
            .success(function (data) {
                $scope.showLoadingMessage = false;
                $scope.layout = data;
            });

        $scope.submit = function (layout) {
            $scope.saving = true;
            $http.post(WebServiceUrl + 'config/layout', layout)
                .success(function (data) {
                    $scope.saving = false;
                    $scope.layout = data;

                    $scope.UpdateCustomLayout(data);
                });
        };
    }]);