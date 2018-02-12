if (jQuery)
    $.ajaxSetup({
        cache: false
    });

    angular.module('MyApp.ctrl.wrkmaint')
    .controller('reportsController', [
            '$scope', '$rootScope', function ($scope, $rootScope) {

                var date = new Date();
                $scope.Report = {
                    RptType: '',
                    UserType: 'Processor',
                    IsGenerating: false,
                    RptDate:'',
                    TeamName: '',
                    EndDate: ''
                };



                $scope.errorMessages = [];
                $scope.rptTimetaken = function () {
                    $scope.Report.RptType = 'Timetaken';
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: '/FinTracker/usr/getList',
                        success: function (data) {
                            $scope.UsersList = data.lst;
                            if (!data._success) {
                                $rootScope.Message = data._success + '|' + data._message;
                            }
                            $scope.$apply();
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                };

                $scope.setTeam = function (team) {
                    $scope.Report.TeamName = team;
                };
                $scope.generateTimetaken = function () {
                    /*$.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: '/FinTracker/rv/set_reviewers',
                    success: function (data) {
                    $scope.UsersList = data.lst;
                    if (!data._success) {
                    $rootScope.Message = data._success + '|' + data._message;
                    }
                    $scope.$apply();
                    },
                    error: function (data) {
                    console.log(data);
                    }
                    });*/
                };

                $scope.rptBetweenDates = function () {
                    $scope.Report.RptType = 'BetweenDts';
                    $scope.clear();
                };

                $scope.rptHistory = function () {
                    $scope.Report.RptType = 'History';
                    $scope.clear();
                };

                $scope.clear = function () {
                    $scope.Report.RptDate = '';
                    $scope.Report.EndDate = '';
                    $scope.Report.UserType = 'Processor';
                    $scope.Report.IsGenerating = false;
                    $scope.errorMessages = [];
                    $("#btnGenerate").removeAttr('disabled');
                };

                $scope.toggleGenerateBtn = function (element) {
                    if ($("#spanplay").text() !== 'Generate') {
                        $("#iconplay").removeClass('glyphicon-globe').addClass("glyphicon-play");
                        $("#spanplay").text('Generate');
                        $("#btnGenerate").removeAttr('disabled');
                        $scope.clear();
                    }
                    else {
                        $("#iconplay").removeClass('glyphicon-play').addClass("glyphicon-globe");
                        $("#spanplay").text('Generating');
                        $("#btnGenerate").attr('disabled', 'disabled');
                    }
                };

                $scope.validateDates = function () {
                    $scope.errorMessages = [];

                    if (isNaN(Date.parse($scope.Report.RptDate))) {
                        $scope.errorMessages.push("Start Date should be entered");
                    }
                    if ($scope.Report.RptType !== 'History') {
                        if (isNaN(Date.parse($scope.Report.EndDate))) {
                            $scope.errorMessages.push("End Date should be entered");
                        }
                        if (Date.parse($scope.Report.RptDate) > Date.parse($scope.Report.EndDate)) {
                            $scope.errorMessages.push("End date should be greater than the Start Date");
                        }
                    }
                    return $scope.errorMessages;
                };

                $scope.generateBetweenDates = function () {
                    $.ajax({
                        url: '/FinTracker/reports/generateFile',
                        data: JSON.stringify($scope.Report),
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        success: function (data) {
                            if (data.success) {
                                $scope.Report.IsGenerating = false;
                                $scope.downloadFile('/FinTracker/reports/getHistory?fileName=' + data.fileName);
                            }
                        }
                    });
                };


                ///Cookie need to be enabled for using this once that is done we can look into this..4th May 2017
                //Once download happened then we can enable the button.
                $scope.downloadFile = function (filename) {
                    $.fileDownload(filename,
                        {
                            httpMethod: "POST",
                            data: {},
                            successCallback: $scope.toggleGenerateBtn
                        });
                };

                $scope.generate = function () {
                    $scope.Report.IsGenerating = true;
                    //We are using this generate function for generating all the reports.
                    if ($scope.Report.RptType !== 'History') {
                        $scope.generateBetweenDates();
                        return;
                    }
                    $scope.toggleGenerateBtn();

                    $scope.History = $scope.Report;
                    $.ajax({
                        url: '/FinTracker/reports/generateFile',
                        data: JSON.stringify($scope.Report),
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        success: function (data) {
                            if (data.success) {
                                $scope.Report.IsGenerating = false;
                                $scope.downloadFile('/FinTracker/reports/getHistory?fileName=' + data.fileName);
                                //window.location = '/FinTracker/reports/getHistory?fileName=' + data.fileName;
                            }
                        }
                        //                        ,
                        //                        complete: function () { //once the report got generated then we need to take the screen back to that earlier state.
                        //                            window.setTimeout(function () { $scope.toggleGenerateBtn(); }, 2000);
                        //                        }
                    });
                };
            }
            ]);