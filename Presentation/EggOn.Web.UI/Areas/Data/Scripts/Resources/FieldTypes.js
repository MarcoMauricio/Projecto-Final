angular.module('EggOn.Data')

    .factory('FieldTypes', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'data/fieldtypes');
    } ]);