angular.module('EggOn.Context')

    .controller('ListDocumentsController', ['$scope', '$state', '$stateParams', '$modal', 'Documents',
    function ($scope, $state, $stateParams, $modal, Documents) {
        console.log('documents controller');

        $scope.loading = true;

        $scope.documents = Documents.query({}, function () {
            $scope.loading = false;
        });

        $scope.deleteDocument = function (document) {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Delete Context', body: 'Are you sure you want to delete this context?' }; } }
            }).result.then(function (id) {
                Documents.remove({ documentId: document[id] }, function (deleteDocument) {
                    $scope.$emit('document::removed', deleteDocument);
                    $scope.records.splice($scope.documents.indexOf(document), 1);
                });
            });
        };

    }]);