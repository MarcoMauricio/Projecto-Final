angular.module('EggOn.Files', ['EggOn.Routes'])
    .config(['$stateProvider', function ($stateProvider) {
        
        $stateProvider

            .state('manager.files', {
                url: '/files',
                abstract: true,
                templateUrl: '/Areas/Files/Views/Index.html',
                controller: 'FilesController'
            })

                .state('manager.files.dashboard', {
                    url: '',
                    templateUrl: '/Areas/Files/Views/Dashboard.html'
                })

                .state('manager.files.create', {
                    url: '/create',
                    templateUrl: '/Areas/Files/Views/CreateRepository.html',
                    controller: 'FilesCreateController'
                })

                .state('manager.files.details', {
                    url: '/{repositoryId}',
                    templateUrl: '/Areas/Files/Views/ViewRepository.html',
                    controller: 'FilesViewController'
                })
        
    }]);
