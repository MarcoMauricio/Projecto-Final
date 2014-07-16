angular.module('EggOn.Context', ['EggOn.Routes'])
    .config(['$stateProvider', function ($stateProvider) {

        $stateProvider
            .state('manager.context', {
                url: '/context',
                abstract: false,
                templateUrl: '/Areas/Context/Views/Index.html',
                controller: 'ListDocumentsController'
            })
            .state('manager.context.dashboard', {
                url: '',
                templateUrl: '/Areas/Context/Views/Dashboard.html'
            })
            .state('manager.context.details', {
                url: '/{documentId}',
                templateUrl: '/Areas/Context/Views/ViewContext.html',
                controller: 'FilesViewController'
        })

    }]);