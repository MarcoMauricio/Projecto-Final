angular.module('EggOn.Context')

    .controller('ListDocumentsController', ['$scope', '$state', '$stateParams', '$modal', 'Documents',
    function ($scope, $state, $stateParams, $modal, Documents) {
        console.log('data details controller');

        $scope.loading = true;

        $scope.documents = Document.Query()

        

        });
    }]);