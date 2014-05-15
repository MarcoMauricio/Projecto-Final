angular.module('EggOn.Files')

    .directive('file', ['$http', function ($http) {

        var uploadFile = function (file, successCallback, errorCallback) {
            var formData = new FormData();
            formData.append('file', file);

            var method = 'POST';
            var url = WebServiceUrl + 'files/';

            $http({ method: method, url: url, data: formData, headers: { 'Content-Type': undefined }, transformRequest: angular.identity })
                .success(function (data, status, headers, config) {
                    successCallback && successCallback(data);
                })
                .error(function (data, status, headers, config) {
                    errorCallback && errorCallback(data);
                });
        };

        var getFile = function (fileId, successCallback, errorCallback) {
            var method = 'GET';
            var url = WebServiceUrl + 'files/' + fileId;

            $http({ method: method, url: url, headers: { 'Content-Type': undefined }, transformRequest: angular.identity })
                .success(function (data, status, headers, config) {
                    successCallback && successCallback(data);
                })
                .error(function (data, status, headers, config) {
                    errorCallback && errorCallback(data);
                });
        };


        return {
            restrict: 'E',

            require: '^ngModel',

            templateUrl: '/Areas/Files/Views/Controls/File.html',

            scope: {},

            link: function ($scope, $elem, $attr, $ngModel) {
                $scope.formats = $attr.formats || '*/*';
                $scope.readonly = $attr.readonly || false;

                $scope.file = null;

                $scope.$watch(function () {
                    return $ngModel.$modelValue;
                }, function (modelValue) {
                    console.log('file', modelValue);
                    if (modelValue) {
                        $scope.file = null;
                        getFile(modelValue, function (file) {
                            $scope.file = file;
                        });
                    } else {
                        $scope.file = null;
                    };
                });

                $scope.loading = false;

                $scope.downloadFile = function (file) {
                    window.open(WebServiceUrl + 'download/' + file.Id + '/' + file.Name, '_blank');
                };

                $scope.clear = function (file) {
                    $scope.file = null;
                    $ngModel.$setViewValue(null);
                };

                var fileEl = $elem.find('input[type="file"]');
                fileEl.on('change', function (e) {
                    console.log('file change');
                    if (this.files.length != 0) {
                        var browserFile = this.files[0];

                        $scope.loading = true;
                        uploadFile(browserFile, function (file) {
                            $scope.file = file;
                            $ngModel.$setViewValue(file.Id);
                            $scope.loading = false;
                        }, function () {
                            $scope.loading = false;
                        });

                        $scope.$apply();
                    }
                });

                $scope.$on('$destroy', function () {
                    fileEl.off('change');
                    console.log('terminated');
                });
            }
        };
    }]);