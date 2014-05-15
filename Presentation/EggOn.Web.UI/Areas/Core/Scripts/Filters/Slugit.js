angular.module('EggOn.Core')

    .filter('slugit', function () {
        return function (text, length, end) {
            if (!text || text.length == 0)
                return;

            //Add any other pair of hardcoded character to translate
            var replaces = { 'á': 'a', 'ã': 'a', 'é': 'e', 'í': 'i', 'ó': 'o', 'ú': 'u', 'ü': 'u', 'ç': 'c' }; 

            var ascii = [];
            var c;

            text = text.toLowerCase().replace(/./g, function (s, key) {
                return replaces[s] || s;
            });

            for (var i = 0; i < text.length; i++) {
                if ((c = text.charCodeAt(i)) < 0x80) {
                    ascii.push(String.fromCharCode(c));
                }
            }

            text = ascii.join("");
            text = text.replace(/[^\w\s-]/g, "").trim().toLowerCase();

            return text.replace(/[-\s]+/g, "-");
        };
    });