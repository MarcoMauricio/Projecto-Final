angular.module('EggOn.Context')

    .factory('Documents', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'context', {}, {});
    }]);
