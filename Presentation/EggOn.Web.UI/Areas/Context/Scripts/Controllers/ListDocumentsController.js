angular.module('EggOn.Context')

    .controller('ListDocumentsController', ['$scope', '$state', '$stateParams', '$modal', 'Documents',
    function ($scope, $state, $stateParams, $modal, Documents) {
        console.log('documents controller');

        $scope.loading = true;

        $scope.documents = Documents.query({}, function () {
            $scope.loading = false;
        });

    }]);