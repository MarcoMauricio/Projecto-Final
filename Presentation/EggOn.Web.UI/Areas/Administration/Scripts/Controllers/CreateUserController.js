angular.module('EggOn.Administration')

    .controller('AdminUsersCreateController', [
    '$scope', '$state', 'Users', 'Roles',
    function ($scope, $state, Users, Roles) {
        console.log('admin users controller');

        $scope.changePageTitle('Users - Administration');

        $scope.user = {};
        $scope.rolesOfUser = [];

        $scope.roleToAdd = null;
        $scope.roles = Roles.query();

        $scope.saving = false;
        $scope.saveSuccess = $scope.saveError = false;

        $scope.submitUser = function (user) {
            $scope.saving = true;
            user.Roles = $scope.rolesOfUser;
            Users.save(user, function (user) {
                $scope.saving = false;
                $scope.saveSuccess = true;
                $state.transitionTo('manager.admin.sections.users.view', { userId: user.Id }, true);
            }, function () {
                $scope.saving = false;
                $scope.saveError = true;
            });
        };

        $scope.userHasRole = function (role) {
            if (!$scope.rolesOfUser)
                return false;

            return $scope.rolesOfUser.find(function (r) { return r.Id == role.Id }) == null;
        };

        $scope.addUserRole = function (role) {
            $scope.rolesOfUser.push(role);
            $scope.roleToAdd = null;
        };

        $scope.removeUserRole = function (role) {
            $scope.rolesOfUser.remove(function (r) { return r.Id == role.Id });
        };

        $scope.goBack = function () {
            $state.transitionTo('manager.admin.sections.users.list', {}, true);
        };
    }]);