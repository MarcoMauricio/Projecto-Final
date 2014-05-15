angular.module('EggOn.Core')

    .filter('offsetTo', function () {
        return function (arr, v) {
            return arr.slice(v);
        };
    });


