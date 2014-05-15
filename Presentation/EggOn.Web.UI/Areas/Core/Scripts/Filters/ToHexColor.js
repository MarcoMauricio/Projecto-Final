angular.module('EggOn.Core')

    .filter('toHexColor', function () {
        return function (text, length, end) {
            if (!text || text.length == 0)
                return;

            var hex = '' + parseInt(text).toString(16);

            // TODO: Pad zeroes.
            return '#' + hex;
        };
    });