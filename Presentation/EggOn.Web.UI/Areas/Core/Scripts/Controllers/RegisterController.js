angular.module('EggOn.Core')

    .controller('RegisterController', ['$scope', '$state', '$location', 'Users', function ($scope, $state, $location, Users) {
        console.log('register controller');

        $scope.changePageTitle('Register');

        $scope.name = '';
        $scope.email = '';
        $scope.password = '';
        $scope.passwordAgain = '';

        $scope.showSpinner = false;
        $scope.showValidationMessage = false;
        $scope.showErrorMessage = false;

        $scope.submitRegisterForm = function () {
            $scope.showSpinner = true;

            var newUser = {
                Name: $scope.name,
                Email: $scope.email,
                Password: $scope.password
            };

            Users.save(newUser, function (response) {
                $scope.showSpinner = false;
                console.log(arguments);
                if (response.MessageType == "Validation") {
                    // Email validation is required.
                    console.log('validation is required');
                    $scope.showValidationMessage = true
                } else {
                    // User was created, login now.
                    $scope.$on('auth::login', function (event, user) {
                        $state.transitionTo('manager.dashboard');
                    });

                    $scope.loginUser($scope.email, $scope.password, false);
                }
            }, function (result) {
                $scope.showSpinner = false;
                $scope.showErrorMessage = true;
                if (result.data.Message)
                    $scope.errorMessage = result.data.Message;
                console.log('error creating user', result.status, result.data);
            });
        };

        $scope.cancelRegisterForm = function () {
            $location.url('/').replace();
            $state.transitionTo('login', {}, true);
        };

        $scope.$on('auth::login', function (event) {
            $scope.showSpinner = false;
            $scope.showError = false;
            $scope.errorMessage = '';
        });
    } ]);