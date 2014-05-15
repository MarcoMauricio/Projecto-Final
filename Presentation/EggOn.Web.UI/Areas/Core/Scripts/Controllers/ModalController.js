angular.module('EggOn.Core')

    .controller('ModalController', ['$scope', '$modalInstance', 'data',
    function ($scope, $modalInstance, data) {
        $scope.headerText = data.header || '';
        $scope.bodyText = data.body || '';

        $scope.okLabel = data.cancel;
        if (data.ok === undefined)
            $scope.okLabel = 'OK';

        $scope.cancelLabel = data.cancel;
        if (data.cancel === undefined)
            $scope.cancelLabel = 'Cancel';

        $scope.ok = function () {
            $modalInstance.close('ok');
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
} ]);