if (jQuery)
    $.ajaxSetup({
        cache: false
    });

angular
    .module('MyApp.ctrl.actmaint', [])
    .controller('actmaintController',[
        '$scope', function ($scope) {
            
           $scope.WorkMaint = {
                WorkitemId:'',
                ActivityId:'',
                ActivityName:'',
                InvoiceNumber: '',
                ItemType: '',
                Comments: '',
                UserId: '',
                StartedDate: '',
                Status:'',
                ActivityList: []
            }
            //$scope.load;

            $scope.load = function () {
                /*Get the all active activities from the table*/
                $.ajax({type:"GET",
                        contentType :'application/json; charset=utf-8',
                        url:'/FinTracker/Activity/List?IsActive=1',
                        success: function (data) {
                        //$scope.WorkMaintList = data.WorkMaintList;
                        $scope.WorkMaint.ActivityList = data.lst;
                        $scope.$apply();
                        }
                        });

//                $.ajax({
//                    type: 'GET',
//                    contentType: 'application/json; charset=utf-8',
//                    url: '/wm/getList',
//                    success: function (data) {
//                        $scope.WorkMaintList = data.WorkMaintList;
//                        $scope.WorkMaint.ActivityList = data.ActivityList;
//                        $scope.$apply();
//                    }
//                });
            }
            $scope.load();

            $scope.clear = function () {
                $scope.WorkMaint.WorkItemId = '';
                $scope.WorkMaint.ActivityId = '';
                $scope.WorkMaint.InvoiceNumber = '';
                $scope.WorkMaint.ItemType = '';
                $scope.WorkMaint.Comments = '';
                $scope.WorkMaint.UserId = '';
                $scope.WorkMaint.StartedDate = '';
                $scope.WorkMaint.Status = '';
            };
           
            $scope.save = function () {
                console.log($scope.WorkMaint)
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WorkMaint),
                    url: '/FinTracker/wm/save',
                    success: function (data, status) {
                        console.log(data)
                        $scope.clear();
                        $scope.load();
                    },
                    error: function (status) { }
                });
            };

            $scope.delete = function (data) {
                var state = confirm("Do you want to delete this record");
                if (state == true) {
                    $.ajax({
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        //data: "{ id: "+data.id+" }",
                        data: JSON.stringify(data),
                        url: '/FinTracker/wm/delete',
                        success: function (status) {
                            console.log(status)
                            $scope.load();
                        },
                        error: function (status) { }
                    });
                }
            }

            $scope.edit = function (index) {
                $scope.WorkMaint.WorkItemId = index.WorkItemId;
                $scope.WorkMaint.ActivityId = index.ActivityId;
                $scope.WorkMaint.InvoiceNumber = index.InvoiceNumber;
                $scope.WorkMaint.ItemType = index.ItemType;
                $scope.WorkMaint.Comments = index.Comments;
                $scope.WorkMaint.UserId = index.UserId;
                $scope.WorkMaint.StartedDate = index.StartedDate;
                $scope.WorkMaint.Status = index.Status;
            };

            $scope.update = function () {                
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.workmaint),
                    url: '/FinTracker/wm/update',
                    success: function (data, status) {
                        console.log(data)
                        $scope.clear();
                        $scope.load();
                    },
                    error: function (status) { }
                });
            };
        }
    ]);

