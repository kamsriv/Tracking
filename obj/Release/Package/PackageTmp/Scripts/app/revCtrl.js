if (jQuery)
    $.ajaxSetup({
        cache: false
    });

    angular.module('MyApp.ctrl.rvmaint', [])
        .controller('rvController', [
            '$scope', '$rootScope', function ($scope, $rootScope) {
                var delay = 1000;
                $scope.ReviewAssign =
                {
                    ReviewerId: '',
                    ReviewerName: '',
                    RevieweeId: [],
                    RevieweeName: '',
                    ReviewerIsPrimary: ''
                };

                $scope.load = function () {
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: '/FinTracker/rv/GetReviewers',
                        success: function (data) {
                            $scope.Users = data.AvailableUsers;
                            $scope.ReviewersList = data.lst;
                            if (!data._success) {
                                $scope.Message = data._message;
                            }
                            $scope.$apply();
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });

                };

                $scope.load();
                $scope.loadReviewee = function () {
                    $scope.RevieweeList = [];
                    for (var i = 0; i < $scope.ReviewersList.length; i++) {
                        if ($scope.ReviewAssign.ReviewerId !== $scope.ReviewersList[i].UserId)
                            $scope.RevieweeList.push($scope.ReviewersList[i]);
                    }
                };

                $scope.save = function () {
                    $.ajax({
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify($scope.ReviewAssign),
                        url: '/FinTracker/rv/save',
                        success: function (data, status) {
                            $rootScope.Message = data._success + "|" + data._message;
                            $scope.$apply();

                            if (data._success)//If success then we need to move to the submit screen.
                            {
                                jQuery("#errMsgs").fadeIn().fadeOut(delay, function () {
                                    $scope.clear();
                                    $scope.load();
                                });
                            }
                            else {
                                jQuery("#errMsgs").fadeIn(delay);
                            }
                        },
                        error: function (status) { }
                    });
                };

                /*we are using this for deleting the items*/
                $scope.edit = function (item) {
                    $.ajax({
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify(item),
                        url: '/FinTracker/rv/delete',
                        success: function (data, status) {
                            $rootScope.Message = data._success + "|" + data._message;
                            $scope.$apply();

                            if (data._success)
                            {
                                jQuery("#errMsgs").fadeIn().fadeOut(delay, function () {
                                    $scope.clear();
                                    $scope.load();
                                });
                            }
                            else {
                                jQuery("#errMsgs").fadeIn(delay);
                            }
                        },
                        error: function (status) { }
                    });
                }

                $scope.clear = function () {
                    $scope.ReviewAssign.RevieweeId = [];
                    $scope.ReviewAssign.ReviewerId = '';
                    $scope.ReviewAssign.ReviewerIsPrimary = '';
                    $scope.RevieweeList = [];
                }

            }
            ]);