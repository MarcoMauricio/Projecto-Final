angular.module('EggOn.Files')

    .directive('fileView', function () {
        return {
            restrict: 'E',
            scope: {
                repositoryId: '=',
                selectedFile: '='
            },

            templateUrl: '/Areas/Files/Views/Controls/FileView.html',

            link: function ($scope, $elem, $attr, $ctrl) {

                $('html').on("keyup.fileView", function (e) {
                    if ($scope.selectedFile != null && e.keyCode == 46) { // delete
                        $scope.deleteFile($scope.selectedFile);
                        $scope.$apply();
                    }
                });

                $('html').on('click.fileView', function (e) {
                    if ($('body').hasClass('modal-open') || $(e.target).parents().is('file-view')) {
                        return;
                    }

                    $scope.selectedFile = null;
                    $scope.$apply();
                });

                $scope.$on("$destroy", function () {
                    $('html').off('keyup.fileView');
                    $('html').off('click.fileView');
                });
            },

            controller: ['$scope', '$modal', '$attrs', '$http', 'Repositories', 'Files',
            function ($scope, $modal, $attrs, $http, Repositories, Files) {
                $scope.breadcrumbs = [];
                $scope.files = [];

                $scope.currentDirectory = null;

                $scope.openFile = function (file) {
                    if (file && file.Type == 1) {
                        window.open(WebServiceUrl + 'download/' + file.Id);
                    } else if (!file || file.Type == 2) {

                        $scope.currentDirectory = file;

                        if (file) {
                            var i = $scope.breadcrumbs.findIndex(function (f) { return f.Id == file.Id; })
                            if (i != null) {
                                $scope.breadcrumbs.splice(i + 1, $scope.breadcrumbs.length);
                            } else {
                                $scope.breadcrumbs.push(file);
                            }
                        } else {
                            $scope.breadcrumbs = [];
                        }

                        showFiles($scope.currentDirectory);
                    }
                };

                $scope.selectItem = function (item) {
                    if (item == $scope.selectedFile)
                        $scope.selectedFile = null;
                    else
                        $scope.selectedFile = item;
                };

                $scope.createFolder = function () {
                    var repositoryId = $scope.repositoryId;
                    var parentFileId = ($scope.currentDirectory) ? $scope.currentDirectory.Id : null;

                    $modal.open({
                        templateUrl: '/Areas/Files/Views/Modals/CreateFolder.html',
                        controller: ['$scope', '$modalInstance', function ($scope, $modalInstance) {
                            $scope.folder = {
                                Name: 'New Folder'
                            };

                            $scope.cancel = function () {
                                $modalInstance.dismiss('cancel');
                            };

                            $scope.createFolder = function () {
                                var folder = {
                                    Name: $scope.folder.Name,
                                    Type: 2, // Folder
                                    Size: 0,
                                    ContentType: null,
                                    RepositoryId: repositoryId,
                                    ParentFileId: parentFileId
                                };

                                Files.save(folder, function () {
                                    $modalInstance.close('ok');
                                });
                            };
                        }]
                    }).result.then(function () {
                        showFiles($scope.currentDirectory);
                    });
                };

                $scope.uploadFiles = function (el) {
                    var fd = new FormData()

                    var input = el;
                    //var files = $('input[type="file"][name="file"]')[0].files;
                    var files = input.files;

                    for (var i = 0; i < files.length; i++)
                        fd.append("file", files[i]);

                    var xhr = new XMLHttpRequest();
                    xhr.upload.addEventListener('progress', function () {
                        console.log('progress');
                    }, false)

                    xhr.addEventListener('load', function () {
                        console.log('load');
                        showFiles($scope.currentDirectory);
                        $scope.$apply();
                    }, false);

                    xhr.addEventListener('error', function () {
                        console.log('error');
                    }, false);

                    xhr.addEventListener('abort', function () {
                        console.log('abort');
                    }, false);

                    xhr.open('POST', WebServiceUrl + 'files?repositoryId=' + $scope.repositoryId + (($scope.currentDirectory) ? '&parentFileId=' + $scope.currentDirectory.Id : ''));
                    xhr.setRequestHeader('Authorization', 'Basic ' + localStorage['auth:token']);
                    xhr.send(fd);
                };

                $scope.renameFile = function (file) {
                    if (!file)
                        return;

                    $modal.open({
                        templateUrl: '/Areas/Files/Views/Modals/RenameFile.html',
                        controller: ['$scope', '$modalInstance', function ($scope, $modalInstance) {
                            $scope.file = angular.copy(file);

                            $scope.cancel = function () {
                                $modalInstance.dismiss('cancel');
                            };

                            $scope.renameFile = function () {
                                Files.update({ fileId: file.Id }, $scope.file, function () {
                                    $modalInstance.close('ok');
                                });
                            };
                        }]
                    }).result.then(function () {
                        showFiles($scope.currentDirectory);
                    });
                };

                $scope.deleteFile = function (file) {
                    if (!file)
                        return;

                    var title = 'Delete File';
                    var msg = 'Are you sure you want to delete "' + file.Name + '"?';

                    $modal.open({
                        templateUrl: '/Areas/Core/Views/Partials/Modal.html',
                        controller: 'ModalController',
                        resolve: { data: function () { return { header: title, body: msg }; } }
                    }).result.then(function () {
                        if ($scope.selectedFile == file)
                            $scope.selectedFile = null;

                        Files.remove({ fileId: file.Id }, function () {
                            showFiles($scope.currentDirectory);
                        });
                    });
                };

                var showFiles = function (parentDirectory) {
                    if (parentDirectory != null) {
                        $scope.files = Files.query({ parentFileId: parentDirectory.Id });
                    } else {
                        $http.get(WebServiceUrl + 'repositories/' + $scope.repositoryId + '/files/')
                            .success(function (files) {
                                $scope.files = files;
                            });
                    }
                };

                $scope.$watch('repositoryId', function (newValue, oldValue) {
                    if (newValue) {
                        $scope.repository = Repositories.get({ repositoryId: newValue }, function (repository) {
                            showFiles(null);
                        });
                    }
                });
            }]
        };
    })