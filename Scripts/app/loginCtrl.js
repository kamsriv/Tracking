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
           $rootScope.UnsavedData = false;

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

            //29Jan2018 expense team
            $scope.ExpCompletionStatus = [{key:1, value:'Completed', activities:[11, 10, 73, 83]}, 
                                          {key:2, value:'Pending', activities:[11, 10, 73]}, 
                                          {key:3, value:'OnHold', activities:[11, 10]},
                                          {key:4, value:'Others', activities:[11, 10]}];

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
                
               $scope.Team = $location.$$url.replace("/","") == ""?teamName: $location.$$url.replace("/",""); //when multiple teams are there then user might select one team so getting that information from location bar.
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
                window.setTimeout(function(){ $scope.getItemsByStatus("Rework"); },1000);
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
                           case 'Rework': $scope.WorkMaintList_rework = data.WorkMaintList;
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
                       
                        if(data._success)//If success then we need to move to the submit screen.
                        {
                            jQuery("#errMsgs").fadeIn(delay).fadeOut(delay,function(){
                                $scope.clear();
                                $scope.load();  
                            });
                        }
                        else //changed on 2Feb2018
                        {
                            if(data._message.indexOf("have") > 0)
                            {
                                $rootScope.Message = "";
                                swal({title:"Error",text:data._message, type:"error",confirmButtonClass: "btn-danger",confirmButtonText: "Try Another"})
                            }
                            else
                            {
                                jQuery("#errMsgs").fadeIn(delay); //if error is bigger then layout is changing
                            }
                        }
                         $scope.$apply();
                    },
                    error: function (status) { },
                    complete: function(){jQuery('BUTTON').removeAttr('disabled');}
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
                        $rootScope.UnsavedData = true; //26Jan2018
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
                        $rootScope.UnsavedData = false; //26Jan2018
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
            
            /*charting control callbackFun*/ 
                $scope.showChart = function(chrtType){
                var postData = {filterInfo:{Filters:[{key:"team",value:$scope.Team}]}};
                    $.ajax({
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(postData),
                    url: '/FinTracker/charts/getChartsData',
                    success: function (data, status) {
                        var lsts = data.split('|');
                        //buggy code it might throw exceptions --> Jan 2nd 2018
                        $scope.ProcData = lsts[0];
                        $scope.Activities = lsts[1];
                        $scope.ProcActData = lsts[2];

                        $scope.RevData = lsts[3];
                        $scope.RevActivities = lsts[4];
                        $scope.RevActData = lsts[5];

                        $scope.$apply();
                    },
                    error: function (status) {
                        console.log(status);
                     },
                    complete: function(){
                        google.charts.load('current',{
                                     'packages': ['corechart']
                                     });
                        
                        switch(chrtType){
                            case "proc":
                                google.charts.setOnLoadCallback(drawProcessorCharts);
                                excolProcChart(document.getElementById('spn_proc'))
                                break;
                            case "rev":
                                google.charts.setOnLoadCallback(drawReviewerCharts);
                                excolRevChart(document.getElementById('spn_rev'))
                                break;
                                }
                        }
                  });
                };
                
               //http://andyshora.com/promises-angularjs-explained-as-cartoon.html
               // $scope.myData = $scope.showChart();

            /*end charts*/

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
                        case "submit": if($scope.WorkMaint.InvoiceNumber == '' && ($scope.WorkMaint.ActivityId==73 || $scope.WorkMaint.ActivityId==11 || $scope.WorkMaint.ActivityId==10 || $scope.WorkMaint.ActivityId==83 || $scope.WorkMaint.ActivityId==78))
                                       {//29Jan2018 invoice number may not be there for most of them it is required when AP Employee Expenses/Concur Expense reports/Emails
                                         $scope.errorMessages.push($scope.WorkMaint.ActivityId==78 ? "Enter Purchase Order Number" : "Exponse report ID is required");
                                       }
                                       if($scope.WorkMaint.RequestNumber == '' && ($scope.WorkMaint.ActivityId==73 || $scope.WorkMaint.ActivityId==11 || $scope.WorkMaint.ActivityId==10 || $scope.WorkMaint.ActivityId==83))
                                       {
                                          $scope.errorMessages.push("Completion Status is required");
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