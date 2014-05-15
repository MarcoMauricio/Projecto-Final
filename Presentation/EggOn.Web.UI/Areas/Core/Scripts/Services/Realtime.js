angular.module('EggOn.Core')

    .service('$realtime', ['$rootScope', function ($rootScope) {
        var proxy = null;

        var initialize = function () {
            //Getting the connection object
            connection = $.hubConnection(window.WebServiceUrl);

            //Creating proxy
            this.proxy = connection.createHubProxy('EggOnHub');

            connection.stateChanged(function (e) {
                if (e.newState === $.signalR.connectionState.connected) {
                    $rootScope.$broadcast("realtime::connected");
                }

                if (e.newState === $.signalR.connectionState.disconnected) {
                    $rootScope.$broadcast("realtime::disconnected");
                }

                if (e.newState === $.signalR.connectionState.reconnecting) {
                    $rootScope.$broadcast("realtime::reconnecting");
                }

                if (e.newState === $.signalR.connectionState.disconnecting) {
                    $rootScope.$broadcast("realtime::disconnecting");
                }

                if (!$rootScope.$$phase) {
                    $rootScope.$apply();
                }
            });

            var retrier = null;

            connection.disconnected(function (e) {
                setTimeout(function () {
                    connection.start();
                }, 5000); // Restart connection after 5 seconds.
            });

            //Starting connection
            connection.start();

            //Publishing an event when server pushes a greeting message
            this.proxy.on('newIssue', function (issue) {
                $rootScope.$broadcast("realtime::issues::new", issue);
            });
        };

        /*
        var sendRequest = function () {
            //Invoking greetAll method defined in hub
            this.proxy.invoke('greetAll');
        };
        */

        return {
            initialize: initialize //,
            //sendRequest: sendRequest
        };
    }]);