if (jQuery)
    $.ajaxSetup({
        cache: false
    });

    /*

    // EXAMPLE PUBLISHER
angular.module('test', []).controller('CtrlPublish', ['$rootScope', '$scope',
  function($rootScope, $scope) {

    $scope.send = function() {
      $rootScope.$broadcast('topic', 'hello');
    };

  }
]);

// EXAMPLE SUBSCRIBER
angular.module('test').controller('ctrlSubscribe', ['$scope',
  function($scope) {

    $scope.$on('topic', function(event, arg) {
      $scope.receiver = 'got your ' + arg;
    });

  }
]);
 <div ng-controller="CtrlPublish">
      <button ng-click="send()">Send</button>
    </div>
    
    <div ng-controller="ctrlSubscribe">
      {{receiver}}
    </div>
*/

 angular.module('MyApp.ctrl.wrkmaint', [])
        .controller('workmaintController',[
        '$scope', '$location','$rootScope', function ($scope,$location,$rootScope) {
            'use strict';
           
           var delay = 1000;

           $scope.LoggedInUser = '';
           $scope.Team = $location.$$url.replace("/","");
           $scope.LockActivity = false;
           
           $scope.toggleLock = function(){
                 $scope.LockActivity = !$scope.LockActivity;
           };

           
           $scope.WorkMaint = {
                WorkItemId:'',
                ActivityId:'',
                ActivityName:'',
                InvoiceNumber: '',
                RequestNumber:'',
                Comments: '',
                UserId: '',
                StartedDate: '',
                Status:'',
                ActivityList: [],
                ReviewerComments:'',
                ReviewerName:'',
                Team: $scope.Team
            }


            $rootScope.$on("updateWorkItem_rev", function (event, args) { //One way of doing the broadcast and catch with in the module and 2 controllers
                //how we will know for which reviewee this count need to be broadcasted.?
               
                if ($scope.LoggedInUser == args.RevieweeId) {
                     $rootScope.RowCount = args.rewcnt;
                }
            });

            
            //$scope.load;
            //Load any item that is at created stage if not there then empty controls
            //If the user don't have permission then how to do that.
            $scope.load = function (userId, teamName) {           
               $scope.LoggedInUser = userId;
               $scope.Team = teamName;
               $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/wm/getList',
                    data:{'team': $scope.Team},
                    success: function (data) {
                        $scope.WorkMaint.ActivityList = data.Activities.lst;
                        
                        if(!data._success)//If error then show error.
                        {
                            $rootScope.Message = data._success +"|"+ data._message;
                            jQuery("#errMsgs").fadeIn(delay);
                        }

                        //If there is any item which is having created status then.
                        /*2nd December 2016 Kalyani asked users can create multiple items at a time.*/
                        $scope.WorkMaintList = data.WorkMaintList;
                        if(data.WorkMaintList.length==1)
                        {
                            $scope.edit(data.WorkMaintList[0]);
                        }

                        $scope.$apply();
                    }
                });
                //Here set the rework count on the header by calling a method.
              if($scope.Team === 'ar'){
                window.setTimeout(function(){ $scope.getItemsByStatus("Rework"); },100);
              }
             //$scope.getItemsByStatus("Created");
            }
            //$scope.load();

           
            //To get the items which are having specific status.
            $scope.getItemsByStatus = function(status)
            {
                 $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    data:{'mStatus':status},
                    url: '/FinTracker/wm/loadItemByStatus',
                    success: function (data) {
                        
                        switch(status)
                        {
                           case 'Rework': $scope.WorkMaintList = data.WorkMaintList;
                                    break;
                           case 'Created': $scope.WorkMaintList_create = data.WorkMaintList;
                                    break;
                           default: $scope.WorkMaintList = data.WorkMaintList;
                                    break;
                        }
                        
                        if(!data._success)//If error then show error.
                        {
                            $rootScope.Message = data._success +"|"+ data._message;//if error is there then only display these load err messages.
                            jQuery("#errMsgs").fadeIn(delay);
                        }

                        $rootScope.RowCount = data.WorkMaintList.length;
                        $scope.$apply();
                    },
                    complete: function(){
                       setTimeout(function(){ $scope.getItemsByStatus("Rework");},30000);
                    }
                });
            };

            $scope.clear = function () {
                $scope.WorkMaint.WorkItemId = '';
              if(!$scope.LockActivity){  //If activity is not locked then clear it otherwise use the same 
                    $scope.WorkMaint.ActivityId = ''; 
                } 
                $scope.WorkMaint.InvoiceNumber = '';
                $scope.WorkMaint.RequestNumber = '';
                $scope.WorkMaint.Comments = '';
                $scope.WorkMaint.UserId = '';
                $scope.WorkMaint.StartedDate = '';
                $scope.WorkMaint.Status = '';
                $rootScope.Message='';
                $scope.errorMessages = [];
                $scope.WorkMaint.ReviewerName = '';

                $scope.$apply();
            };

            $scope.loadAllData = function()
            {
                $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/wm/loadAllData',
                    success: function (data) {
                        $scope.WorkMaintList = data.WorkMaintList;

                        if(!data._success)//If error then show error.
                            $rootScope.Message = data._success +"|"+ data._message;

                        $scope.$apply();
                    }
                });
            };

           
            $scope.save = function () {
                jQuery('BUTTON').attr('disabled','disabled');
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WorkMaint),
                    url: '/FinTracker/wm/save',
                    success: function (data, status) {
                        $rootScope.Message = data._success +"|"+ data._message;
                        $scope.$apply();
                        if(data._success)//If success then we need to move to the submit screen.
                        {
                            jQuery("#errMsgs").fadeIn(delay).fadeOut(delay,function(){
                                jQuery('BUTTON').removeAttr('disabled');
                                $scope.clear();
                                $scope.load();  
                            });
                        }
                        else
                            jQuery("#errMsgs").fadeIn(delay);

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
                $scope.WorkMaint.RequestNumber = index.RequestNumber;
                $scope.WorkMaint.Comments = index.Comments;
                $scope.WorkMaint.UserId = index.UserId;
                $scope.WorkMaint.StartedDate = index.StartedDate;
                $scope.WorkMaint.Status = index.Status;
                $scope.WorkMaint.ReviewerName = index.ReviewerName;
            };

            $scope.update = function () {
                $scope.WorkMaint.Status = 'Submitted';
                jQuery('BUTTON').attr('disabled','disabled');
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WorkMaint),
                    url: '/FinTracker/wm/update',
                    success: function (data, status) {
                        $rootScope.Message = data._success +"|"+ data._message;
                        $scope.$apply();

                        if(data._success)
                        {
                         jQuery("#errMsgs").fadeIn().fadeOut(delay,function(){
                                            $scope.clear();
                                            jQuery('BUTTON').removeAttr('disabled');
                                        });
                        }
                        else
                        jQuery("#errMsgs").fadeIn(delay);
                        
                        
                        $scope.load();                           
                        //after updating the data we need to broadcast message to the reviewer.
                        $rootScope.$emit("updateWorkItem", {revcnt: data.WMCnt});
                    },
                    error: function (status) {
                     console.log(status);
                     }
                });
            };

            $scope.redirectToReviewer = function()
            {    $location.path("#/rv/");};

            /*for rework related things*/

            $scope.clearRedo = function() {
                   $scope.WMRedo.WorkItemId = '';
                   $scope.WMRedo.ActivityId = '';
                   $scope.WMRedo.ActivityName ='';
                   $scope.WMRedo.InvoiceNumber = '';
                   $scope.WMRedo.RequestNumber = '';
                   $scope.WMRedo.Comments = '';
                   $scope.WMRedo.ReviewerId = '';
                   $scope.WMRedo.ReviewerComments = '';
                   $scope.WMRedo.StartedDate = '';
                   $scope.WMRedo.Status = '';
                   $scope.WMRedo.UserId = '';
                   $scope.WMRedo.ReviewerName = '';
            };

            $scope.WMRedo = {
                    WorkItemId : '',
                    ActivityId : '',
                    ActivityName:'',
                    InvoiceNumber:'',
                    RequestNumber:'',
                    Comments:'',
                    ReviewerId:'',
                    ReviewerComments:'',
                    StartedDate:'',
                    Status:'',
                    UserId:'',
                    ReviewerName:''
            };
            $scope.rework = function(index)
            {
                //save a history record to the database. as this is being reworked.
                angular.element("#myModalShower").trigger('click');
                angular.element('#myModal').drags({ handle: ".modal-header" });
                $scope.WMRedo.WorkItemId = index.WorkItemId;
                $scope.WMRedo.ActivityId = index.ActivityId;
                $scope.WMRedo.ActivityName = index.ActivityName;
                $scope.WMRedo.InvoiceNumber = index.InvoiceNumber;
                $scope.WMRedo.RequestNumber = index.RequestNumber;
                $scope.WMRedo.Comments = index.Comments;
                $scope.WMRedo.UserId = index.UserId;
                $scope.WMRedo.StartedDate = index.StartedDate;
                $scope.WMRedo.Status = "Reworking agin"; //index.Status
                $scope.WMRedo.ReviewerComments = index.ReviewerComments;
                $scope.WMRedo.ReviewerName = index.ReviewerName;
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WMRedo),
                    url: '/FinTracker/wm/update',
                    success: function (data, status) {
                        $rootScope.Message = data._success +"|"+ data._message;
                        $scope.$apply();
                    },
                    error: function (status) {
                     console.log(status);
                     }
                });
            };

            $scope.reSubmit = function (itemStatus) {
                $scope.WMRedo.Status = itemStatus;
                $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify($scope.WMRedo),
                    url: '/FinTracker/wm/update',
                    success: function (data, status) {
                        angular.element("#myModalShower").trigger('click');

                        $rootScope.Message = data._success +"|"+ data._message;
                        $scope.$apply();                       
                        
                        //Here set the rework count on the header by calling a method.
                        
                        jQuery("#errMsgs").fadeIn().fadeOut(delay,function(){
                                $scope.clearRedo();//clearing the redo object
                                $scope.getItemsByStatus("Rework");                                    
                            });

                        //after updating the data we need to broadcast message to the reviewer.
                        $rootScope.$emit("updateWorkItem", {revcnt: data.WMCnt, ReviewerId: data.ReviewerId });
                    },
                    error: function (status) {
                     console.log(status);
                     }
                });
                }
            /*end rework*/

            //Validates the form input based on the values
            $scope.validate_ap = function(mode){
            $scope.errorMessages = [];
                        switch(mode)
                        {
                        case "save": if($scope.WorkMaint.ActivityId == '')
                                     {
                                        $scope.errorMessages.push("Activity Id is required");
                                     }
                                        
                        break;
                        case "submit": if($scope.WorkMaint.InvoiceNumber == '' && ($scope.WorkMaint.ActivityId == 7 || $scope.WorkMaint.ActivityId == 9 
                                                || $scope.WorkMaint.ActivityId == 15))
                                       {
                                         $scope.errorMessages.push("Invoice/Requisition number is required");
                                       }
                                       if($scope.WorkMaint.Comments == '' && ($scope.WorkMaint.ActivityId == 8 || $scope.WorkMaint.ActivityId == 13 
                                                || $scope.WorkMaint.ActivityId == 14 || $scope.WorkMaint.ActivityId == 16 ))
                                       {
                                         $scope.errorMessages.push("Comments are required");
                                       }
                        break;
                        }

                        return  $scope.errorMessages;
            };

             $scope.validate_tr = function(mode){
            $scope.errorMessages = [];
                        switch(mode)
                        {
                        case "save": if($scope.WorkMaint.ActivityId == '')
                                     {
                                        $scope.errorMessages.push("Activity Id is required");
                                     }
                                        
                        break;
                        case "submit": if($scope.WorkMaint.InvoiceNumber == '' && ($scope.WorkMaint.ActivityId == 20))
                                       {
                                         $scope.errorMessages.push("Accouont Name/Number is required");
                                       }
                        break;
                        }

                        return  $scope.errorMessages;
            };


            $scope.validate_exp = function(mode){
            $scope.errorMessages = [];
                        switch(mode)
                        {
                        case "save": if($scope.WorkMaint.ActivityId == '')
                                     {
                                        $scope.errorMessages.push("Activity Id is required");
                                     }
                                        
                        break;
                        case "submit": if($scope.WorkMaint.InvoiceNumber == '')
                                       {
                                         $scope.errorMessages.push("Exponse report ID is required");
                                       }
                        break;
                        }

                        return  $scope.errorMessages;
            };

            $scope.validate_save = function() { //Professional Angularjs bk
                var validationFunctions = [
                    {
                            fn: function() {
                            return !!$scope.WorkMaint.ActivityId
                            },
                        message: 'Activity is required'
                    },
                    {
                           fn: function() {
                            return !!$scope.WorkMaint.InvoiceNumber || !!$scope.WorkMaint.RequestNumber
                            },
                        message: 'Invoice Number or Request Number required'
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

            $scope.validate_update = function(){
                $scope.errorMessages = [];
                if(!$scope.WorkMaint.Comments)
                {
                 $scope.errorMessages.push('Comments are required');
                }
                return $scope.errorMessages;
            };

             $scope.isFormValid = function() //Used by resubmit button.
             {
                if($scope.validate_save().length>0) //display validations in a modal window.
                {
                    return false;
                }
                return true;
             };

             $scope.validate_resubmit = function() {
                var validationFunctions = [
                    {
                            fn: function() {
                            return !!$scope.WMRedo.ActivityId
                            },
                        message: 'Activity is required'
                    },
                    {
                           fn: function() {
                            return !!$scope.WMRedo.InvoiceNumber || !!$scope.WMRedo.RequestNumber
                            },
                        message: 'Invoice Number or Request Number required'
                    },
                    {
                           fn: function() {
                            return !!$scope.WMRedo.Comments || !!$scope.WMRedo.Comments
                            },
                        message: 'Comments are required'
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