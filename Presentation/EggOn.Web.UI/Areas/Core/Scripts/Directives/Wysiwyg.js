angular.module('EggOn.Core')

    .directive('wysiwyg', ['$parse', function () {
        var generatedIds = 0;
        return {
            restrict: "A",
            require: "ngModel",

            //templateUrl: '/Areas/Core/Views/Controls/Editor.html',

            link: function (scope, elm, attrs, ngModel) {
                var expression, options, tinyInstance;

                // generate an ID if not present
                if (!attrs.id) {
                    attrs.$set('id', 'uiTinymce' + generatedIds++);
                }

                attrs.$set('style', 'margin-bottom: 10px; width:100%;');

                options = {
                    // Update model on button click
                    onchange_callback: function (inst) {
                        if (inst.isDirty()) {
                            inst.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        }
                    },
                    // Update model on keypress
                    handle_event_callback: function (e) {
                        if (this.isDirty()) {
                            this.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        }
                        return true; // Continue handling
                    },
                    // Update model when calling setContent (such as from the source editor popup)
                    setup: function (ed) {
                        ed.on('init', function (e) {
                            var ed = e.target;
                            ngModel.$render();
                        });

                        ed.on('change', function (e) {
                            var ed = e.target;
                            if (ed.isDirty()) {
                                ed.save();
                                ngModel.$setViewValue(elm.val());
                                if (!scope.$$phase) {
                                    scope.$apply();
                                }
                            }
                        });
                    },
                    mode: 'exact',
                    elements: attrs.id,
                    statusbar : false,
                    plugins: "autoresize wordcount",

                    theme: "modern" //,
                    //theme_advanced_toolbar_location: "top",
                    //theme_advanced_toolbar_align: "left",
                    //theme_advanced_statusbar_location: "bottom",
                    //theme_advanced_resizing: true,
                };

                if (attrs.uiTinymce) {
                    expression = scope.$eval(attrs.uiTinymce);
                } else {
                    expression = {};
                }

                angular.extend(options, expression);

                setTimeout(function () {
                    tinymce.init(options);
                });


                ngModel.$render = function () {
                    if (!tinyInstance) {
                        tinyInstance = tinymce.get(attrs.id);
                    }
                    if (tinyInstance) {
                        tinyInstance.setContent(ngModel.$viewValue || '');
                    }
                };

                //$elm.find('textarea').each(function (i, el) {
                /*
                var el = $elm.find('textarea');

                var editorIndex = tinyMCE.editors.length;
                var lateData = '';

                el.tinymce({
                script_url: "/Scripts/tinymce/tiny_mce.js",

                // General options
                theme: "advanced",
                plugins: "pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

                // Theme options
                theme_advanced_buttons1: "save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
                theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
                theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: true,

                // Example content CSS (should be your site CSS)
                //content_css: "css/example.css",

                // Drop lists for link/image/media/template dialogs
                //template_external_list_url: "js/template_list.js",
                //external_link_list_url: "js/link_list.js",
                //external_image_list_url: "js/image_list.js",
                //media_external_list_url: "js/media_list.js",

                // Replace values for the template plugin
                //template_replace_values: {
                //    username: "Some User",
                //    staffid: "991234"
                //}
                oninit: function () {
                tinyMCE.editors[editorIndex].setContent(lateData);
                ngModel.$render = function () {
                // TODO: Fix this active editor.
                tinyMCE.editors[editorIndex].setContent(ngModel.$viewValue);
                };
                },

                onchange_callback: function () {
                var newValue = tinyMCE.editors[editorIndex].getContent();
                if (newValue !== ngModel.$viewValue) {
                ngModel.$setViewValue(newValue);
                $scope.$apply();
                }
                }
                });

                ngModel.$render = function () {
                lateData = ngModel.$viewValue;
                };
                */
            }
        };
    } ]);