angular.module('EggOn.Core')

    .filter('datetime', function () {
        return function (input, format) {
            if (input) {
                return moment.utc(input).format(format);
            } else
                return '';
        };
    });