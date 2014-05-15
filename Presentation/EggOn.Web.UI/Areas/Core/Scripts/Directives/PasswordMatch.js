angular.module('EggOn.Core')

    .directive('passwordMatch', function () {
        return {
            restrict: 'A',
            require: "ngModel",
            link: function ($scope, $element, $attrs, $ctrl) {
                // if ngModel is not defined, we do nothing
                if (!$ctrl) {
                    return;
                }

                if (!$attrs['passwordMatch']) {
                    return;
                }

                $scope.$watch($attrs.passwordMatch, function (value) {
                    // the second value is not set yet, we do nothing
                    if ($ctrl.$viewValue === undefined || $ctrl.$viewValue === '') {
                        return;
                    }

                    $ctrl.$setValidity('passwordMatch', value === $ctrl.$viewValue);
                });

                $ctrl.$parsers.push(function (value) {
                    var isValid = value === $scope.$eval($attrs.passwordMatch);
                    $ctrl.$setValidity('passwordMatch', isValid);
                    return isValid ? value : undefined;
                });
            }
        };
    });