if (jQuery)
    $.ajaxSetup({
        cache: false
    });

    angular.module('MyApp.ctrl.usrmaint', [])
    .controller('usrController', [
        '$scope', function ($scope, $routeParams) {

            $scope.User = {
                UserId: '',
                UserName: '',
                IsActive: '',
                EmailId: '',
                CreatedBy: '',
                CreatedDate: '',
                IsAdmin: '',
                IsReviewer: ''
            }

            $scope.get = function () {
                $.ajax({
                    type: "GET",
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/usr/getList',
                    success: function (data) {
                        $scope.UserList = data.lst;
                        $scope.$apply();
                    }
                });
            }

            $scope.get();

            $scope.save = function () {
                console.log($scope.User)
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.User),
                    url: '/FinTracker/usr/Save',
                    success: function (data, status) {
                        $scope.Message = data._message;
                        $scope.$apply();

                        if (data._success) {
                            $scope.clear();
                            $scope.get();
                            window.setTimeout(function () { //clear the message after certain amount of time.
                                $scope.Message = '';
                            }, 2000);
                        }

                    },
                    error: function (status) { }
                });
            };

            $scope.edit = function (index) {
                $scope.User.UserId = index.UserId;
                $scope.User.UserName = index.UserName;
                $scope.User.EmailId = index.EmailId;
                $scope.User.CreatedDate = index.CreatedDate;
                $scope.User.CreatedBy = index.CreatedBy;
                $scope.User.IsAdmin = index.IsAdmin;
                $scope.User.IsActive = index.IsActive;
                $scope.IsEdit = true;
            };

            $scope.update = function () {
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.User),
                    url: '/FinTracker/usr/update',
                    success: function (data, status) {
                        if (data._success)//If success then we need to allow for the new entry
                        {
                            $scope.clear();
                        }
                        $scope.Message = data._message;
                        $scope.$apply();
                        $scope.get();
                    },
                    error: function (status) { }
                });
            };

            $scope.clear = function () {
                $scope.User.UserId = '';
                $scope.User.UserName = '';
                $scope.User.IsActive = '';
                $scope.User.EmailId = '';
                $scope.User.CreatedDate = '';
                $scope.User.CreatedBy = '';
                $scope.User.IsAdmin = '';
                $scope.IsEdit = false;
                $scope.errorMessages = [];
            };

            $scope.tableRowHide = function (cnt) {

            };

            $scope.Delete = function (user) {
                if (user.IsReviewer) {
                    //display a confirmation box to move his reviewees to another reviewer.
                    angConfirm('hello');
                }
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ data: user.UserId }),
                    url: '/FinTracker/usr/delete',
                    success: function (data) {
                        if (data._success)//If success then we need to allow for the new entry
                        {
                            $scope.clear();
                        }
                        $scope.Message = data._message;
                        $scope.$apply();
                        $scope.get();
                    },
                    error: function (status) { }
                });
            };

            //Validates the form input based on the values
            $scope.validate_save = function () { //Professional Angularjs bk
                var validationFunctions = [
                    {
                        fn: function () {
                            return !!$scope.User.UserId
                        },
                        message: 'User Id is required'
                    },
                    {
                        fn: function () {
                            return !!$scope.User.UserName
                        },
                        message: 'User Name is required'
                    },
                    {
                        fn: function () {
                            return !!$scope.User.EmailId
                        },
                        message: 'Email Id is required'
                    }
                ];
                $scope.errorMessages = [];
                for (var i = 0; i < validationFunctions.length; ++i) {
                    if (!validationFunctions[i].fn()) {
                        $scope.errorMessages.push(validationFunctions[i].message);
                    }
                }
                return $scope.errorMessages;
            };
            $scope.validate_update = function () { //Professional Angularjs bk
                var validationFunctions = [
                    {
                        fn: function () {
                            return !!$scope.User.UserName
                        },
                        message: 'User Name is required'
                    },
                    {
                        fn: function () {
                            return !!$scope.User.EmailId
                        },
                        message: 'Email Id is required'
                    }
                ];
                $scope.errorMessages = [];
                for (var i = 0; i < validationFunctions.length; ++i) {
                    if (!validationFunctions[i].fn()) {
                        $scope.errorMessages.push(validationFunctions[i].message);
                    }
                }
                return $scope.errorMessages;
            };
        }
    ]);

