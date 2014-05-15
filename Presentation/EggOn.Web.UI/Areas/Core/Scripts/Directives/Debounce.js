angular.module('EggOn.Core')

/*!
 * jQuery Orangevolt Ampere
 *
 * version : 0.2.0
 * created : 2013-08-15
 * source  : https://github.com/lgersman/jquery.orangevolt-ampere
 *
 * author  : Lars Gersmann (lars.gersmann@gmail.com)
 * homepage: http://www.orangevolt.com
 *
 * Copyright (c) 2013 Lars Gersmann; Licensed MIT, GPL
 */
.directive('ngDebounce', ['$timeout', function ($timeout) {
    var TIMEOUT = 400;

    return {
        restrict: 'A',
        require: 'ngModel',
        priority: 99,
        link: function (scope, elm, attr, ngModelCtrl) {
            if (attr.type === 'radio' || attr.type === 'checkbox') return;

            elm.unbind('input');

            var debounce;
            elm.bind('input', function () {
                $timeout.cancel(debounce);
                debounce = $timeout(function () {
                    scope.$apply(function () {
                        ngModelCtrl.$setViewValue(elm.val());
                    });
                }, TIMEOUT);
            });
            elm.bind('blur', function () {
                scope.$apply(function () {
                    ngModelCtrl.$setViewValue(elm.val());
                });
            });
        }
    };
}]);