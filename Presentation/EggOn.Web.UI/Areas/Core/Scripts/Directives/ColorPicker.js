angular.module('EggOn.Core')

    .directive('colorPicker', function() {
        return {
            restrict: 'EA',
            require: '?ngModel',
            link: function ($scope, $element, $attrs, $ngModel) {

                var processChange = function (e) {
                    if ($scope.$$phase === '$digest')
                        return;

                    $scope.$apply(function (scope) {
                        if (e.color !== undefined) {
                            $ngModel.$setViewValue(e.color.toHex());
                        } else {
                            $ngModel.$setViewValue('');
                        }
                    });
                };

                $element.colorpicker().on('changeColor', processChange);

                $scope.$watch(function () {
                    return $ngModel.$modelValue;
                }, function (modelValue) {
                    if (modelValue) {
                        $element.colorpicker('setValue', modelValue)
                    } else {
                        $element.colorpicker('setValue', '');
                    };
                });

                // ColorPicker plugin doesn't support destroy? Weird.
                /*
                $scope.$on('$destroy', function () {
                    $element.colorpicker('destroy');
                });
                */
            }
        };
    });