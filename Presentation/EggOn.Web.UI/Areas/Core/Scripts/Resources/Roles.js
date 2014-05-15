angular.module('EggOn.Core')

    .factory('Roles', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'roles/:roleId', {
            roleId: '@Id'
        }, {
            update: { method: 'PUT' }
        });
    } ]);


