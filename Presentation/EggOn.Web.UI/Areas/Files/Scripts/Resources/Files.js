angular.module('EggOn.Files')

    .factory('Files', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'files/:fileId', {
            fileId: '@Id'
        }, {
            update: { method: 'PUT' }
        });
    } ]);


