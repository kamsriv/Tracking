angular
    .module('MyApp', [
    'ngRoute',
    'MyApp.ctrl.wrkmaint',
    'MyApp.ctrl.usrmaint',
    'MyApp.ctrl.actmaint',
    'MyApp.ctrl.rvmaint'
])


    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider
        .when('/', {
            templateUrl: '/Fintracker/wm/list',
            controller: 'workmaintController'
        })
        .when('/ar', {
            templateUrl: '/Fintracker/wm/ArTeam',
            controller: 'workmaintController'
        })
        .when('/ap', {
            templateUrl: '/Fintracker/wm/APTeam',
            controller: 'workmaintController'
        })
        .when('/ex', {
            templateUrl: '/Fintracker/wm/ExpTeam',
            controller: 'workmaintController'
        })
        .when('/tr', {
            templateUrl: '/Fintracker/wm/TrTeam',
            controller: 'workmaintController'
        })
        .when('/fp', {
            templateUrl: '/Fintracker/wm/FPTeam',
            controller: 'workmaintController'
        })
        .when('/rv', {
            templateUrl: '/Fintracker/review/list',
            controller: 'reviewerController'
        })
        .when('/u', {
            templateUrl: '/Fintracker/usr/list',
            controller: 'usrController'
        })
        .when('/e', {
            templateUrl: '/Fintracker/wm/viewEntries',
            controller: 'workmaintController'
        })
        .when('/rp', {
            templateUrl: '/Fintracker/reports/list',
            controller: 'reportsController'
        })
        .when('/r', {
            templateUrl: '/Fintracker/rv/list',
            controller: 'rvController'
        });
        $locationProvider.html5Mode({
            enabled: false, //Disabling the HTML5 mode (now in Hashbang mode) so the browser can understand the url route.https://docs.angularjs.org/guide/$location
            requireBase: true
        });

    } ]).directive('tooltip', function () { //tooltip for angular http://stackoverflow.com/questions/20666900/using-bootstrap-tooltip-with-angularjs
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.hover(function () {
                    // on mouseenter
                    element.tooltip('show');
                }, function () {
                    // on mouseleave
                    element.tooltip('hide');
                });
            }
        };
    })
    .directive('angConfirm', function (msg) {
        return function (scope, element, attributes) {
            return confirm(msg);
        }
        /*{restrict: 'E',
        templateUrl: ''} we can call another page which have the modal popup*/

    })
    .filter("jsDate", function () {
        return function (x) {
            if (x != null && x.length > 0)
                return new Date(parseInt(x.substr(6)));
        };
    });

//simple table from https://github.com/tendentious/Angular-Simple-Table may be we can replace with ng-table.

    