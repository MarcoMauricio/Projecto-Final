angular.module('EggOn.Core')

    .controller('RecoverPasswordController', ['$scope', '$state', '$location', '$http', function ($scope, $state, $location, $http) {
        console.log('recover controller');

        $scope.changePageTitle('Recover');

        $scope.email = '';
        $scope.password = '';
        $scope.passwordAgain = '';

        $scope.showSpinner = false;
        $scope.showMessage = false;

        $scope.key = ($location.search()).key;

        $scope.submitRecoverForm = function () {
            $scope.showSpinner = true;

            $http({ method: 'POST', url: WebServiceUrl + 'users/recover2?key=' + $scope.key + '&password=' + $scope.password })
            .success(function (data, status, headers, config) {
                $scope.showSpinner = false;
                $scope.message = '<strong>Password changed!</strong><br /> The password was changed, you can now login.';
                $scope.showMessage = true;
            });
        };

        $scope.submitEmailForm = function () {
            $scope.showSpinner = true;

            $http({ method: 'POST', url: WebServiceUrl + 'users/recover1?email=' + $scope.email })
            .success(function (data, status, headers, config) {
                $scope.showSpinner = false;
                $scope.message = '<strong>Email sent!</strong><br /> Email sent to reset your password.';
                $scope.showMessage = true;
            });
        };

        $scope.cancelRecoverForm = function () {
            $location.url('/').replace();
            $state.transitionTo('login', {}, true);
        };
    } ]);