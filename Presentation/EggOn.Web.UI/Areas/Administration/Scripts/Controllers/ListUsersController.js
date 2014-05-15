angular.module('EggOn.Administration')

    .controller('AdminUsersListController', [
    '$scope', '$state', '$modal', 'Users',
    function ($scope, $state, $modal, Users) {
        console.log('admin users controller');

        $scope.changePageTitle('Users - Administration');

        $scope.showLoadingMessage = true;
        $scope.users = Users.query(function () {
            $scope.showLoadingMessage = false;
        });

        $scope.editUser = function (user) {
            $state.transitionTo('manager.admin.sections.users.view', { userId: user.Id }, true);
        };

        $scope.deleteUser = function (user) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete User', body: 'Are you sure you want to delete this user?' }; } }
            }).result.then(function () {
                Users.remove({ userId: user.Id }, function (user) {
                    $scope.users.remove(function (el) { return el.Id == user.Id });
                });
            });
        };
    }]);