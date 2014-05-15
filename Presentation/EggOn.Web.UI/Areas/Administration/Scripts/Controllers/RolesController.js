angular.module('EggOn.Administration')

    .controller('AdminRolesController', [
    '$scope', '$modal', 'Roles',
    function ($scope, $modal, Roles) {
        console.log('admin roles controller');

        $scope.changePageTitle('Roles - Administration');

        $scope.newRole = {};

        $scope.roles = [];

        $scope.showLoadingMessage = true;
        $scope.roles = Roles.query(function (roles) {
            $scope.showLoadingMessage = false;
        });

        $scope.createRole = function () {
            Roles.save($scope.newRole, function (role) {
                $scope.newRole = {};

                var search = $scope.roles.find(function (el) { return el.Id == role.Id });
                if (search == null)
                    $scope.roles.push(role);
            });
        };

        $scope.editRole = function (role) {

        };

        $scope.deleteRole = function (role) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Role', body: 'Are you sure you want to delete role "' + role.Name + '"?' }; } }
            }).result.then(function () {
                Roles.remove({ roleId: role.Id }, function (role) {
                    $scope.roles.remove(function (el) { return el.Id == role.Id });
                });
            });
        };
    }]);