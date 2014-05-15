angular.module('EggOn.Core')

    .controller("ProfileController", ['$scope', '$state', '$auth', 'Users',
    function ($scope, $state, $auth, Users) {
        console.log('profile controller');

        $scope.changePageTitle('Profile');

        $scope.errorMessage = '';

        $scope.saving = $scope.saveSuccess = $scope.saveError = false;
        $scope.oldPassword = $scope.newPassword = $scope.newPasswordVerification = '';

        $scope.checkPasswordChange = function () {
            $scope.saveError = false;

            if (!$auth.checkPassword($scope.oldPassword)) {
                $scope.errorMessage = 'The old password is incorrect.';
                $scope.saveError = true;
                return true;
            }

            if ($scope.newPassword != $scope.newPasswordVerification) {
                $scope.errorMessage = 'The new password verification doesn\'t match.';
                $scope.saveError = true;
                return true;
            }

            return false;
        };

        $scope.changePassword = function () {
            if ($scope.checkPasswordChange())
                return;

            $scope.saving = true;
            $scope.saveSuccess = $scope.saveError = false;

            $auth.changePassword($scope.newPassword, function () {

                $scope.oldPassword = $scope.newPassword = $scope.newPasswordVerification = '';
                $scope.profileForm.$setPristine();

                $scope.saveSuccess = true;
                $scope.saving = false;
            }, function () {
                $scope.saveError = true;
                $scope.errorMessage = 'Failed to change the password on the server. Please try again later.';
                $scope.saving = false;
            });
        };
    } ]);