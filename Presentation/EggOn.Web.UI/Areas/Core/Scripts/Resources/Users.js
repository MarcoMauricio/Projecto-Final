angular.module('EggOn.Core')

    .factory('Users', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'users/:userId', {
            userId: '@Id'
        }, {
            update: { method: 'PUT' }
        });
    } ]);


