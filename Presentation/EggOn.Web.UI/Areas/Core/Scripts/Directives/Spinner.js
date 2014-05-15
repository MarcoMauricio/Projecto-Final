angular.module('EggOn.Core')

    .directive('spinner', function () {
        return {
            require: '?ngModel',
            link: function (scope, element, attrs, controller) {
                var updateModel;

                element.spinner({
                    min: -999999,
                    max: 999999
                });

                element.spinner('value', 0);

                scope.$on('$destroy', function () {
                    //element.spinner('destroy');
                });

                if (controller != null) {
                    updateModel = function (value) {
                        return scope.$apply(function () {
                            return controller.$setViewValue(value);
                        });
                    };

                    controller.$render = function () {
                        if (controller.$viewValue != null)
                            element.spinner('value', controller.$viewValue);

                        return element.on('changed', function (e, v) {
                            if (updateModel) updateModel(v);
                        });
                    };
                }

                return element.on('changed', function (e, v) {
                    if (updateModel) updateModel(v);
                });
            }
        };
    });