angular.module('EggOn.Core')

    .controller("ManagerController", ['$rootScope', '$scope', '$state', '$modal', '$http', '$resource', '$realtime', 'Users', 'Languages',
    function ($rootScope, $scope, $state, $modal, $http, $resource, $realtime, Users, Languages) {
        console.log('manager controller');

        $scope.changePageTitle('Manager');

        $scope.avatarUrl = md5($scope.currentUser.Email.trim().toLowerCase());

        // Default flag.
        $scope.userLanguageCode = 'gb';

        $scope.UserHasRole = function (role) {
            return $scope.currentUser.Roles.find(function (r) { return r.Name.toLowerCase() == role.toLowerCase() }) != null;
        };

        $scope.interfaceLanguages = Languages.query(function (languages) {
            for(var i = 0; i < languages.length; i++) {
                if ($scope.currentUser.InterfaceLanguageId == languages[i].Id) {
                    $scope.userLanguageName = languages[i].Name;
                    $scope.userLanguageCode = languages[i].Code;
                    break;
                }
            }
        });

        $scope.changeLanguage = function (language) {
            Users.update({ userId: $scope.currentUser.Id }, { InterfaceLanguageId: language.Id }, function (user) {
                $scope.currentUser = user;
                $scope.userLanguageName = language.Name;
                $scope.userLanguageCode = language.Code;
                $scope.$broadcast('eggon::changedLanguage', language);
            });
        };

        $scope.askLogoutUser = function () {
            $modal.open({
                templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                controller: 'ModalController',
                resolve: { data: function () { return { header: 'Logout', body: 'Are you sure?' }; } }
            }).result.then(function () {
                $scope.logoutUser();
            });
        };


        // Layout customization:
        $scope.CustomLogoId = null;
        
        $scope.UpdateCustomLayout = function () {
            $http.get(WebServiceUrl + 'config/layout')
                .success(function (data) {
                    console.log('Layout from server', data);

                    $scope.CustomLogoId = data.LogoId;

                    $rootScope.CustomBar = {
                        'background-color': data.BarBackColor,
                        'background-image': 'inherit'
                    };

                    $rootScope.CustomBack = {
                        'background-color': data.BackColor,
                        'background-image': 'inherit'
                    };

                    $rootScope.CustomButtonBack = {
                        'background-color': data.BarButtonColor,
                        'background-image': 'inherit'
                    };
                });
        };
        $scope.UpdateCustomLayout();

        $rootScope.useStyleIf = function (cond, style) {
            return (cond) ? style : {};
        };

        $scope.getLogoImage = function () {
            if ($scope.CustomLogoId == null)
                return '/Areas/Core/Assets/logo.png';
            else
                return WebServiceUrl + '/download/' + $scope.CustomLogoId;
        };

        // SignalR test
        $scope.realtimeConnected = false;

        //$realtime.initialize();

        $scope.$on('realtime::connected', function () {
            console.log('realtime connected :)');
            $scope.realtimeConnected = true;
        });

        $scope.$on('realtime::reconnecting', function () {
            console.log('realtime reconnecting :|');
            $scope.realtimeConnected = false;
        });

        $scope.$on('realtime::disconnected', function () {
            console.log('realtime disconnected :(');
            $scope.realtimeConnected = false;
        });

    } ]);