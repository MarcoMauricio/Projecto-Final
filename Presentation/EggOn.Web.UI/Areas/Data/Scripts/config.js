angular.module('EggOn.Data', ['EggOn.Routes'])
    .config(['$stateProvider', function ($stateProvider) {
        
        $stateProvider

            .state('manager.data', {
                url: '/data',
                abstract: true,
                templateUrl: '/Areas/Data/Views/Index.html',
                controller: 'DataController'
            })

                .state('manager.data.dashboard', {
                    url: '',
                    templateUrl: '/Areas/Data/Views/Dashboard.html'
                })

                .state('manager.data.create', {
                    url: '/create',
                    templateUrl: '/Areas/Data/Views/ViewContainer.html',
                    controller: 'ContainerController'
                })

                .state('manager.data.details', {
                    url: '/{containerId}',
                    abstract: true,
                    template: '<section ui-view autoscroll="false"></section>'
                })

                    .state('manager.data.details.edit', {
                        templateUrl: '/Areas/Data/Views/ViewContainer.html',
                        controller: 'ContainerController'
                    })

                    .state('manager.data.details.showRecords', {
                        url: '',
                        templateUrl: '/Areas/Data/Views/ListRecords.html',
                        controller: 'ListRecordsController'
                    })

                    .state('manager.data.details.createRecord', {
                        url: '/create',
                        templateUrl: '/Areas/Data/Views/ViewRecord.html',
                        controller: 'RecordController'
                    })

                    .state('manager.data.details.editRecord', {
                        url: '/{recordId}',
                        templateUrl: '/Areas/Data/Views/ViewRecord.html',
                        controller: 'RecordController'
                    })
        
    }]);
