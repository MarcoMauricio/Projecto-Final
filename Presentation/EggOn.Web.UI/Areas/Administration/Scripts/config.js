angular.module('EggOn.Administration', ['EggOn.Routes'])

    .config(['$stateProvider', function ($stateProvider) {
        console.log('administration config');

        $stateProvider

            .state('manager.admin', {
                url: '/admin',
                abstract: true,
                template: '<section ui-view autoscroll="false"></section>',
                controller: ['$scope', '$state', function($scope, $state) {
                    if (!$scope.UserHasRole('Administrator')) {
                        $state.transitionTo('manager.dashboard', {}, true);
                    }
                }]
            })

                .state('manager.admin.dashboard', {
                    url: '',
                    templateUrl: '/Areas/Administration/Views/Dashboard.html'
                })

                .state('manager.admin.sections', {
                    url: '',
                    abstract: true,
                    templateUrl: '/Areas/Administration/Views/Index.html',
                    controller: 'AdminController'
                })

                    .state('manager.admin.sections.users', {
                        url: '/users',
                        abstract: true,
                        template: '<section ui-view autoscroll="false"></section>',
                    })

                        .state('manager.admin.sections.users.list', {
                            url: '',
                            templateUrl: '/Areas/Administration/Views/ListUsers.html',
                            controller: 'AdminUsersListController'
                        })

                        .state('manager.admin.sections.users.create', {
                            url: '/create',
                            templateUrl: '/Areas/Administration/Views/ViewUser.html',
                            controller: 'AdminUsersCreateController'
                        })

                        .state('manager.admin.sections.users.view', {
                            url: '/:userId',
                            templateUrl: '/Areas/Administration/Views/ViewUser.html',
                            controller: 'AdminUsersViewController'
                        })

                    .state('manager.admin.sections.roles', {
                        url: '/roles',
                        templateUrl: '/Areas/Administration/Views/ListRoles.html',
                        controller: 'AdminRolesController'
                    })

                    .state('manager.admin.sections.layout', {
                        url: '/layout',
                        templateUrl: '/Areas/Administration/Views/Layout.html',
                        controller: 'AdminLayoutController'
                    })
    }]);
