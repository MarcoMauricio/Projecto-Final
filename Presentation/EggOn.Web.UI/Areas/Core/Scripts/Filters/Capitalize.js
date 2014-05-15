angular.module('EggOn.Core')

    .filter('capitalize', function () {
        return function (text, length, end) {
            if (!text || text.length == 0)
                return;

            return text[0].toUpperCase() + text.substring(1);
        };
    });