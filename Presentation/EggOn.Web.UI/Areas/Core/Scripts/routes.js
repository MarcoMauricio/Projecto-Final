angular.module('EggOn.Routes', [])

    .config(['$locationProvider', '$urlRouterProvider', '$stateProvider',
        function ($locationProvider, $urlRouterProvider, $stateProvider) {

        $locationProvider.html5Mode(true);

        $urlRouterProvider.otherwise('/error');

        $stateProvider

            .state('login', {
                templateUrl: '/Areas/Core/Views/Login.html',
                controller: 'LoginController'
            })

            .state('register-user', {
                url: '/register',
                templateUrl: '/Areas/Core/Views/Register.html',
                controller: 'RegisterController'
            })

            .state('recover-password', {
                url: '/recover',
                templateUrl: '/Areas/Core/Views/Recover.html',
                controller: 'RecoverPasswordController'
            })

            .state('manager', {
                abstract: true,
                templateUrl: '/Areas/Core/Views/Manager.html',
                controller: 'ManagerController'
            })

                .state('manager.dashboard', {
                    url: '/',
                    templateUrl: '/Areas/Core/Views/Dashboard.html'
                })

                .state('manager.profile', {
                    url: '/profile',
                    templateUrl: '/Areas/Core/Views/Profile.html',
                    controller: 'ProfileController'
                })

            .state('error', {
                url: '/error',
                templateUrl: '/Areas/Core/Views/Error.html'
            })
    }]);