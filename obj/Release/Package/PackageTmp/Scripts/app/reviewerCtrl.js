if (jQuery)
    $.ajaxSetup({
        cache: false
    });


    /*var mediator = (function () {
        var subscribe = function (channel, fn) {
            if (!mediator.channels[channel]) mediator.channels[channel] = [];
            mediator.channels[channel].push({ context: this, callback: fn });
            return this;
        },

    publish = function (channel) {
        if (!mediator.channels[channel]) return false;
        var args = Array.prototype.slice.call(arguments, 1);
        for (var i = 0, l = mediator.channels[channel].length; i < l; i++) {
            var subscription = mediator.channels[channel][i];
            subscription.callback.apply(subscription.context, args);
        }
        return this;
    };

        return {
            channels: {},
            publish: publish,
            subscribe: subscribe,
            installTo: function (obj) {
                obj.subscribe = subscribe;
                obj.publish = publish;
            }
        };

    } ());
    */
    angular.module('MyApp.ctrl.wrkmaint')
    .controller('reviewerController', [
        '$scope', '$rootScope', function ($scope, $rootScope) {

            $rootScope.$on("updateWorkItem", function (event, args) { //One way of doing the broadcast and catch with in the module and 2 controllers
                //how we will know for which reviewer this count need to be broadcasted.?
                if ($scope.WMReview.ReviewerId == args.ReviewerId) {
                    $scope.RowCount = args.revcnt;
                    $scope.$apply();
                }
            });

            var delay = 1000;
            $scope.WMReview = {
                WorkItemId: '',
                ActivityId: '',
                ActivityName: '',
                InvoiceNumber: '',
                RequestNumber: '',
                Comments: '',
                UserId: '',
                StartedDate: '',
                Status: '',
                ActivityList: [],
                UsersList: [],
                ReviewerId: '',
                ReviewerComments: '',
                UpdatedDate: '',
                ErrFound: ''
            }

            $scope.WMWaitingReview = [];
            /*simple table integration for filters*/
            //$scope.myTableSettings = new TableSettings();

            $scope.FiltersInfo = {
                Filters: [], //{ Key: '', Value: ''}
                PageIndex: 1,
                PageSize: 100,
                TotalRows: 0,
                TotalPages: 0
            }


            $scope.load = function () {
                //bring the filters which you want to set when loading
                $.ajax({
                    type: "GET",
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/usr/getList',
                    success: function (data) {
                        $scope.WMReview.UserList = data.lst;
                        $scope.$apply();
                    }
                });

                $.ajax({ type: "GET",
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/Activity/List?IsActive=1',
                    success: function (data) {
                        $scope.WMReview.ActivityList = data.lst;
                        $scope.$apply();
                    }
                });

                $scope.filter();
            }


            $scope.filter = function () {
                
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.FiltersInfo),
                    url: '/FinTracker/review/getPending',
                    success: function (data) {
                        $scope.WMWaitingReview = data.WorkMaintList;
                        $scope.selectOptions = [100, 200, 300, 500, 700, 1000];

                        $scope.FiltersInfo.TotalRows = data.WorkMaintList ? data.TotalRows : '';
                        $scope.FiltersInfo.TotalPages = Math.ceil(data.TotalRows / $scope.FiltersInfo.PageSize);

                        if (!data._success)//If error then show error.
                        {
                            $rootScope.Message = data._success + "|" + data._message;
                            jQuery("#errMsgs").fadeIn(delay);
                        }
                        $scope.$apply();
                    },
                    complete: function () {
                        //setTimeout(function () { $scope.load(); }, 300000);
                    }
                });
            }

            $scope.setPage = function (pIndex) {
                $scope.FiltersInfo.PageIndex = pIndex;
                $scope.filter();
            }

            $scope.setRows = function (pSize) {
                $scope.FiltersInfo.PageSize = pSize;
                $scope.filter();
            }

            $scope.clearFilters = function () {
                //clear only filter values not the assigned keys
                for (var i = 0; i < $scope.FiltersInfo.Filters.length; i++)
                    $scope.FiltersInfo.Filters[i].Value = '';

                $scope.filter();
            }

            $scope.load_old = function () {
                $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    //filtersPageInfo: JSON.stringify($scope.FiltersInfo),
                    url: '/FinTracker/review/getPending',
                    success: function (data) {
                        $scope.WMWaitingReview = data.WorkMaintList;

                        // $scope.myTableSettings.setData($scope.WMWaitingReview);
                        $scope.selectOptions = [100, 200, 300, 500, 700, 1000];

                        $scope.RowCount = data.WorkMaintList.length;
                        if (!data._success)//If error then show error.
                        {
                            $rootScope.Message = data._success + "|" + data._message;
                            jQuery("#errMsgs").fadeIn(delay);
                        }
                        $scope.WMReview.ActivityList = data.Activities.lst;
                        $scope.$apply();
                    },
                    complete: function () {
                        setTimeout(function () { $scope.load(); }, 300000);
                    }
                });

            }
            $scope.load();




            $scope.edit = function (index) {/// <reference path="../../Models/" />
                /* http: //stackoverflow.com/questions/16265844/invoking-modal-window-in-angularjs-bootstrap-ui-using-javascript*/
                angular.element('#myModalShower').trigger('click');
                angular.element('#myModal').drags({ handle: ".modal-header" });
                $scope.WMReview.ActivityName = index.ActivityName;
                $scope.WMReview.Comments = index.Comments;
                $scope.WMReview.RequestNumber = index.RequestNumber;
                $scope.WMReview.InvoiceNumber = index.InvoiceNumber;
                $scope.WMReview.UserId = index.UserId;
                $scope.WMReview.StartedDate = index.StartedDate;
                $scope.WMReview.WorkItemId = index.WorkItemId;
                $scope.WMReview.Status = "In Review";
                $scope.WMReview.ErrFound = false;
                $scope.WMReview.ReviewerComments = "";

                //Add an entry to the history saying this is in review.
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WMReview),
                    url: '/FinTracker/review/save',
                    success: function (data, status) {
                        $scope.$apply();
                        //$scope.Message = data._message;
                    },
                    error: function (status) { }
                });

            };

            $scope.clear = function () {
                $scope.WMReview.ActivityName = '';
                $scope.WMReview.Comments = '';
                $scope.WMReview.RequestNumber = '';
                $scope.WMReview.InvoiceNumber = '';
                $scope.WMReview.UserId = '';
                $scope.WMReview.StartedDate = '';
                $scope.WMReview.WorkItemId = '';
                $scope.WMReview.Status = '';
                $scope.WMReview.ErrFound = '';
                $scope.WMReview.ReviewerComments = '';
            };

            $scope.update = function (status) {
                $scope.WMReview.Status = status; //modifying the status here.
                jQuery('BUTTON').attr('disabled', 'disabled');
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WMReview),
                    url: '/FinTracker/review/update',
                    success: function (data, status) {
                        angular.element('#myModalShower').trigger('click'); //close the modal

                        $rootScope.Message = data._success + "|" + data._message;
                        $scope.$apply();
                        if (data._success)//If error then show error.
                        {
                            jQuery("#errMsgs").fadeIn().fadeOut(delay, function () {
                                $scope.filter();
                            });
                        }
                        else {
                            jQuery("#errMsgs").fadeIn(delay);
                        }
                        jQuery('BUTTON').removeAttr('disabled');
                        //after updating the data we need to broadcast message to the reviewee if there is any items need to be reworked.
                        $rootScope.$emit("updateWorkItem_rev", { rewcnt: data.WMCnt, RevieweeId: data.RevieweeId });

                    },
                    error: function (status) { }
                });
            };

            $scope.validate_update = function () {
                $scope.errorMessages = [];
                if (!$scope.WMReview.ReviewerComments) {
                    $scope.errorMessages.push('Comments are required');
                }
                return $scope.errorMessages;
            };

            /*$scope.save = function () {
            console.log($scope.User)
            $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify($scope.User),
            url: '/FinTracker/usr/Save',
            success: function (data, status) {
            $scope.Message = data._message;
            $scope.$apply();
            window.setTimeout(function () {
            if (data._success)//If success then we need to clear the control values.
            {
            $scope.Message = '';
            }
            }, 2000);
            $scope.get();
            $scope.clear();
            },
            error: function (status) { }
            });
            };

           
            $scope.update = function () {
            alert($scope.User);
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
            };*/
        }
    ]);