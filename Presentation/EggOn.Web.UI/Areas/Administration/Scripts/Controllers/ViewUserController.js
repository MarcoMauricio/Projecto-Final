angular.module('EggOn.Administration')

    .controller('AdminUsersViewController', [
    '$scope', '$state', '$stateParams', '$modal', 'Users', 'Roles',
    function ($scope, $state, $stateParams, $modal, Users, Roles) {
        console.log('admin users controller');

        $scope.changePageTitle('Edit User - Users - Administration');

        

        $scope.user = Users.get({ userId: $stateParams.userId }, function (user) {
            $scope.rolesOfUser = angular.copy(user.Roles);
            $scope.userName = user.Name;
        });
        
        $scope.roleToAdd = null;
        $scope.roles = Roles.query();

        $scope.saving = false;
        $scope.saveSuccess = $scope.saveError = false;

        $scope.submitUser = function (user) {
            $scope.saving = true;
            user.Roles = $scope.rolesOfUser;
            Users.update(user, function (user) {
                $scope.saving = false;
                $scope.saveSuccess = true;
                $scope.userName = user.Name;
            }, function () {
                $scope.saving = false;
                $scope.saveError = true;
            });
        };

        $scope.deleteUser = function (user) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete User', body: 'Are you sure you want to delete this user?' }; } }
            }).result.then(function () {
                Users.remove({ userId: user.Id }, function (user) {
                    $state.transitionTo('manager.admin.sections.users.list', {}, true);
                });
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