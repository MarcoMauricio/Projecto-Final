angular.module('EggOn.Core')

    .service('$fileupload', ['$q', '$http', function ($q, $http) {

        this.uploadPackage = function (id, pack) {
            var deferred = $q.defer();

            var method = (id) ? 'PUT' : 'POST';
            var url = WebServiceUrl + 'files/packages/' + ((id) ? id : '');

            var formData = new FormData();

            for (var i = 0; i < pack.Files.length; i++) {
                formData.append('file_' + i, pack.Files[i]);
            }

            $http({ method: method, url: url, data: formData, headers: { 'Content-Type': undefined }, transformRequest: angular.identity })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    deferred.reject(data, status, headers, config);
                });

            return deferred.promise;
        };

        this.getPackage = function (id) {
            if (!id) {
                deferred.reject(null);
                return;
            }

            var deferred = $q.defer();

            $http({ method: 'GET', url: WebServiceUrl + 'files/packages/' + id })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    deferred.reject(data, status, headers, config);
                });

            return deferred.promise;
        };

    }]);