angular.module('EggOn.Core')

    .directive('datetimePicker', ['$parse', function ($parse) {
        return {
            restrict: 'EA',
            require: '?ngModel',
            link: function ($scope, $element, $attrs, $ngModel) {
                $element.addClass('date');

                if ($attrs.ngDisabled !== undefined) {
                    var disabledAccessor = $parse($attrs.ngDisabled);
                }
                
                var processChange = function (e) {
                    if (e.namespace == "")
                        return;

                    if (e.date !== undefined) {
                        $ngModel.$setViewValue(e.date.toDate());
                    } else {
                        $ngModel.$setViewValue('');
                    }

                    if($scope.$$phase != '$apply' && $scope.$$phase != '$digest') {
                        $scope.$apply();
                    }
                };

                if ($attrs.datetimePicker == undefined || $attrs.datetimePicker === '') {
                    $attrs.datetimePicker = 'datetime';
                }

                //var pickDate = $attrs.datetimePicker.indexOf('date') != -1;
                //var pickTime = $attrs.datetimePicker.indexOf('time') != -1;

                $element.datetimepicker({
                    language: 'pt',
                    defaultDate: moment().local(),
                    pick12HourFormat: false,
                    format: 'DD/MM/YYYY HH:mm'
                }).on('dp.change', processChange);

                $scope.$watch(function () {
                    return $ngModel.$modelValue;
                }, function (modelValue) {
                    if (modelValue) {
                        $element.data('DateTimePicker').setDate(moment.utc(modelValue));
                    } else {
                        $element.data('DateTimePicker').setDate(null);
                    };
                });

                if ($attrs.ngDisabled !== undefined) {
                    $scope.$watch(disabledAccessor, function (val) {
                        if (!val) {
                            $element.data('DateTimePicker').enable();
                        } else {
                            $element.data('DateTimePicker').disable();
                        }
                    });
                }

                $scope.$on('$destroy', function () {
                    $element.datetimepicker('destroy');
                });
            }
        };
    }]);