angular.module('EggOn.Data')

    .factory('Containers', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'data/containers/:containerId', {
            containerId: '@Id'
        }, {
            update: { method: 'PUT' }
        });
    } ]);