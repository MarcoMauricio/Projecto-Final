angular.module('EggOn.Context', ['EggOn.Routes'])
    .config(['$stateProvider', function ($stateProvider) {

        $stateProvider

            .state('manager.context', {
                url: '/context',
                abstract: true,
                templateUrl: '/Areas/Context/Views/Index.html',
                controller: 'ContextController'
            })

                .state('manager.context.dashboard', {
                    url: '',
                    templateUrl: '/Areas/Context/Views/Dashboard.html'
                })


    }]);
