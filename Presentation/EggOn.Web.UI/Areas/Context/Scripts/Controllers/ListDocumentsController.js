angular.module('EggOn.Context')

    .controller('ListDocumentsController', ['$scope', '$state', '$stateParams', '$modal', 'Documents',
    function ($scope, $state, $stateParams, $modal, Documents) {
        console.log('documents controller');

        $scope.loading = true;

        $scope.documents = [];
        $scope.documents = Documents.query();
        for (var i = 0; i < $scope.documents.length;i++)
        console.log($scope.documents[i]);

    }]);