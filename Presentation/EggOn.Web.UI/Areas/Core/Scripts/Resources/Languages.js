angular.module('EggOn.Core')

    .factory('Languages', ['$resource', function ($resource) {
        return $resource(WebServiceUrl + 'languages');
    }]);


