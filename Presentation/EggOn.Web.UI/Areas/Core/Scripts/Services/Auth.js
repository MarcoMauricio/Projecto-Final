angular.module('EggOn.Core')

    .factory('$auth', ['$rootScope', '$http', 'Users', function ($scope, $http, Users) {
        var currentUser = null;
        var currentToken = null;

        return {
            isAuthenticated: function () {
                return currentUser != null;
            },

            authenticate: function (username, password) {
                var token = btoa(username + ':' + password);
                $http.defaults.headers.common['Authorization'] = "Basic " + token;

                $http.get(WebServiceUrl + 'users/current')
                .success(function (data, status, headers, config) {
                    if (data.Name == undefined || data.Email == undefined) {
                        localStorage['auth:token'] && delete localStorage['auth:token'];
                        localStorage['auth:user'] && delete localStorage['auth:user'];
                        delete $http.defaults.headers.common['Authorization'];
                        $scope.$broadcast('auth::failed', data, status, headers);
                        return;
                    }

                    localStorage['auth:token'] = token;
                    localStorage['auth:user'] = username;

                    currentUser = data;
                    currentToken = token;

                    $scope.$broadcast('auth::login', currentUser, currentToken);
                })
                .error(function (data, status, headers, config) {
                    localStorage['auth:token'] && delete localStorage['auth:token'];
                    localStorage['auth:user'] && delete localStorage['auth:user'];
                    delete $http.defaults.headers.common['Authorization'];
                    $scope.$broadcast('auth::failed', data, status, headers);
                });
            },

            deauthenticate: function () {
                localStorage['auth:token'] && delete localStorage['auth:token'];

                $http.defaults.headers.common['Authorization'] && delete $http.defaults.headers.common['Authorization'];

                currentUser = null;
                currentToken = null;

                $scope.$broadcast('auth::logout');
            },

            checkPassword: function (password) {
                var token = btoa(currentUser.Email + ':' + password);
                return currentToken == token;
            },

            changePassword: function (newPassword, callbackSuccess, callbackError) {
                Users.get({ userId: currentUser.Id }, function (user) {
                    user.Password = newPassword;
                    Users.update({ userId: user.Id }, user, function (user) {
                        var token = btoa(user.Email + ':' + newPassword);
                        $http.defaults.headers.common['Authorization'] = "Basic " + token;

                        currentUser = user;
                        currentToken = token;

                        callbackSuccess && callbackSuccess(user);
                    }, function () {
                        callbackError && callbackError();
                    });
                }, function () {
                    callbackError && callbackError();
                });
            },

            getUser: function () {
                return currentUser;
            },

            getToken: function () {
                return currentToken;
            },

            hasStoredAuth: function () {
                return !!localStorage['auth:token'];
            },

            getStoredUsername: function () {
                return localStorage['auth:user'] || null;
            },

            useStoredAuth: function (callback) {
                if (localStorage['auth:token'] == undefined) {
                    return false;
                }

                this.authenticate.apply(this, atob(localStorage['auth:token']).split(':'));
                return true;
            }
        };
    } ]);