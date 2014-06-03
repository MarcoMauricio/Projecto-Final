angular.module('EggOn.Core', [ ]);

angular.module('EggOn.Core', [
    'ui.router', 'ui.bootstrap', 'ui.bootstrap.pagination', 'ngSanitize', 'ngResource', 'EggOn.Routes'

    , 'EggOn.Administration'

    // Place custom modules namespaces here.
    , 'EggOn.Files'
    , 'EggOn.Data'
    , 'EggOn.Context'
])

    .config(['$modalProvider', function ($modalProvider) {

        $modalProvider.options = {
            backdrop: true,
            keyboard: true // close with esc key
        };

    }])

    .run(['$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams) {

        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;

        console.info('EggOn CMS is running.');

    } ]);