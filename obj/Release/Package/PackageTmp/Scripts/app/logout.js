
angular
    .module('MyApp.ctrl.logout', [])
    .controller('logoutController', [
        '$scope',
        function ($scope) {

            alert("In Logout")
        }
    ]);