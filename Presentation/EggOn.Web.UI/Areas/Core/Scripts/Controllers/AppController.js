angular.module('EggOn.Core')

    .controller("AppController", ['$scope', '$state', '$location', '$urlRouter', '$auth',
    function ($scope, $state, $location, $urlRouter, $auth) {
        console.log('app controller');

        $scope.currentUser = null;
        $scope.pageTitle = '';

        $scope.changePageTitle = function (title) {
            $scope.pageTitle = title;
        };

        $scope.loginUser = function (username, password) {
            $auth.authenticate(username, password);
        };

        $scope.logoutUser = function () {
            $auth.deauthenticate();
        };

        $scope.$on('$stateChangeStart', function (e, toState, toParams, fromState, fromParams) {
            if (!$auth.isAuthenticated() && toState.name.indexOf('manager') == 0) {
                e.preventDefault();

                $state.transitionTo('login', {}, false);
            }
        });

        $scope.$on('$stateChangeSuccess', function (e) {
            console.log('changed state', arguments);
        });

        $scope.$on('$stateChangeError', function (e) {
            console.log('error while changing state', arguments);
        });

        $scope.$on('auth::login', function (event, user) {
            console.log('login confirmed', user);

            $scope.currentUser = user;

            // Get state based on url.
            $urlRouter.sync()
        });

        $scope.$on('auth::logout', function (event) {
            console.log('logout confirmed');

            $state.transitionTo('login', {}, false);
        });
    } ]);