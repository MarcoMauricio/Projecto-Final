angular.module('EggOn.Core')

    .directive('formGroup', function () {
        return {
            restrict: 'C',
            require: '^form',
            scope: {},
            link: function (scope, element, attrs, formController) {

                if (!formController)
                    return;

                var inputName = element.find('[name]').first().attr('name');

                if (!inputName || formController[inputName] === undefined)
                    return;
                
                scope.$watch(function () {
                    return formController[inputName].$valid;
                }, function (valid) {
                    if (formController.$pristine || formController[inputName].$pristine) {
                        element.removeClass('has-error has-success');
                    } else if (valid) {
                        element.addClass('has-success');
                        element.removeClass('has-error');
                    } else {
                        element.removeClass('has-success');
                        element.addClass('has-error');
                    }
                });
            }
        };
    });