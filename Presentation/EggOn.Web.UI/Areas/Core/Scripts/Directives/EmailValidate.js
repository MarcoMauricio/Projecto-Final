angular.module('EggOn.Core')

    .directive('emailValidate', function () {
        return {
            restrict: 'A',

            require: "ngModel",

            // scope = the parent scope
            // elem = the element the directive is on
            // attr = a dictionary of attributes on the element
            // ctrl = the controller for ngModel.
            link: function (scope, elem, attr, ctrl) {
                console.log('emailValidate directive');
                var validateFn = function (value) {
                    var re = /^\S+@\S+$/; // Simplest client-side email validation.
                    ctrl.$setValidity('emailValidate', value == '' || re.test(value));
                    return value;
                };
                ctrl.$formatters.push(validateFn);
                ctrl.$parsers.push(validateFn);

                validateFn(ctrl.$viewValue);
            }
        };
    });