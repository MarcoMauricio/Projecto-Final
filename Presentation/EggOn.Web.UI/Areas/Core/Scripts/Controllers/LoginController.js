angular.module('EggOn.Core')

    .controller('LoginController', ['$scope', '$state', '$location', '$http', '$auth',
    function ($scope, $state, $location, $http, $auth) {
        console.log('login controller');

        $scope.changePageTitle('Login');

        $scope.showLoginForm = false;

        $scope.username = $auth.getStoredUsername() || '';
        $scope.password = '';

        $scope.showSpinner = false;
        $scope.showValidation = false;
        $scope.showError = false;
        $scope.errorMessage = '';

        var key = ($location.search()).key;
        if (key) {
            $scope.showSpinner = true;

            $http({ method: 'POST', url: WebServiceUrl + 'users/validate?key=' + key })
            .success(function (data, status, headers, config) {
                $scope.showSpinner = false;
                $scope.showValidation = true;
            })
            .error(function (data, status, headers, config) {
                $scope.showSpinner = false;
                $scope.showValidation = false;
                $scope.showError = true;
                $scope.errorMessage = 'Failed to validate that user.';
            });
        }

        $scope.submitLoginForm = function () {
            $scope.showSpinner = true;
            $scope.loginUser($scope.username, $scope.password);
        };

        $scope.showRegisterForm = function () {
            $state.transitionTo('register-user', {}, true);
        };

        $scope.showNotYetForm = function () {
            $state.transitionTo('register-notyet', {}, true);
        };

        $scope.showRecoverForm = function () {
            $state.transitionTo('recover-password', {}, true);
        };

        $scope.$on('auth::login', function (event) {
            $scope.showSpinner = false;
            $scope.showError = false;
            $scope.showValidation = false;
            $scope.showLoginForm = false;
            $scope.username = '';
            $scope.password = '';
        });

        $scope.$on('auth::failed', function (event, data, status, headers) {
            $scope.showSpinner = false;
            $scope.showError = true;

            if (status == 401)
                $scope.errorMessage = 'Login failed.';
            else
                $scope.errorMessage = 'There was a error while connecting to the server. (' + status + ')';

            $scope.password = '';
        });

        // Auto-Login
        if ($auth.hasStoredAuth() && $auth.useStoredAuth()) {
            $scope.username = $auth.getStoredUsername();
            $scope.password = '*****';
            $scope.showSpinner = true;
            $scope.showLoginForm = true;
        } else {
            $scope.showLoginForm = true;
        }

        // Auto-Focus
        if ($scope.username == '') {
            $('input[name="username"]').focus();
        } else {
            $('input[name="password"]').focus();
        }
} ]);