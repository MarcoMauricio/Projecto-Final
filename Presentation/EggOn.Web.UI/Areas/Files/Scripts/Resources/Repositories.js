angular.module('EggOn.Files')

    .factory('Repositories', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'repositories/:repositoryId', {
            repositoryId: '@Id'
        }, {
            update: { method: 'PUT' }
        });
    } ]);


