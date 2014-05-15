angular.module('EggOn.Core')

    .directive('formControlFeedback', function () {
        return {
            restrict: 'C',
            require: '^form',
            link: function (scope, element, attrs, formController) {

                if (!formController)
                    return;

                var inputElement = element.siblings('[name]').first();

                if (!inputElement)
                    return;

                var inputName = inputElement.attr('name');

                if (!inputName || formController[inputName] === undefined)
                    return;
                
                element.addClass('fa');

                if (inputElement.attr('required'))
                    element.addClass('fa-asterisk');

                scope.$watch(function () {
                    return formController[inputName].$valid;
                }, function (valid) {

                    if (formController.$pristine || formController[inputName].$pristine) {
                        element.removeClass('fa-asterisk fa-times fa-check');
                    } else if (valid) {
                        element.removeClass('fa-asterisk fa-times');
                        element.addClass('fa-check');
                    } else {
                        element.removeClass('fa-asterisk fa-check');
                        element.addClass('fa-times');
                    }
                });
            }
        };
    });