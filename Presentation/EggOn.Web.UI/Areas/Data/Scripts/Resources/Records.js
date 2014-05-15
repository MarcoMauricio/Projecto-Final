angular.module('EggOn.Data')

    .factory('Records', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'data/containers/:containerId/records/:recordId', {}, {
            update: { method: 'PUT' }
        });
    } ]);


