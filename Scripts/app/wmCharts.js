
function excolProcChart(obj) {
    var screenHt = screen.availHeight, screenWidth = screen.availWidth;
    var normalHt, normalWidth;
    var constHundred = 120;
    normalHt = normalWidth = 0;

    //if clicked when it is minimized.
    if ($(obj).hasClass("popout")) {
        $("#processorContainer").removeClass("shorten").addClass("enlarge").css("height", screenHt);
        //$(obj).children(0).removeClass("glyphicon-resize-full").addClass("glyphicon-resize-small");
        $(obj).removeClass("popout").addClass("popin");
        $(obj).parent().css("margin-left", "0px");
        $("#chart_div_p").css({ "height": screenHt - constHundred, "width": screenWidth/2 });
        $("#chart_div_pAct").css({ "height": screenHt - constHundred, "width": screenWidth / 2 });
        drawProcessorCharts(); //re-drawing the charts to fill the height and width.
    }
    else {
        $("#processorContainer").removeClass("enlarge").addClass("shorten").css("height", normalHt);
        //$(obj).children(0).removeClass("glyphicon-resize-small").addClass("glyphicon-resize-full");
        $(obj).removeClass("popin").addClass("popout");
        $(obj).parent().css("margin-left", "-15px");
        $("#chart_div_p").css({"height": normalHt, "width": normalWidth });
        $("#chart_div_pAct").css({"height": normalHt, "width": normalWidth });
    }
}

function drawProcessorCharts() {
    drawProcessor();
    drawProcessorByAct();
}

function drawProcessor() {
    if (document.getElementById('hdnProcessor').value == "") return;
    var procData = [['Actul Utilization', 'Target Hrs', 'Total Time Spent']];
    //from hidden fields we will get array or arrays as data so we need to push each array to the above
    var hdnData = eval('[' + document.getElementById('hdnProcessor').value + ']');
    for (var arr in hdnData) {
        procData.push(hdnData[arr]);
    }
    // Some raw data (not necessarily accurate)

    var data = google.visualization.arrayToDataTable(procData);

    var options = {
        title: 'Daily utilization',
        vAxis: { title: 'Total Hours' },
        hAxis: { title: 'Actual Utilization' },
        seriesType: 'bars',
        series: { 0: { type: 'line'} }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_div_p'));
    chart.draw(data, options);
}

//for fullsize chart example see https://stackoverflow.com/questions/20764157/zoom-google-line-chart/36596155
function drawProcessorByAct() {
    if (document.getElementById('hdnProcessorAct').value == "") return;

    var procActData = [];
    var hdnAct = eval('[' + document.getElementById('hdnActivities').value + ']');
    var hdnData = eval('[' + document.getElementById('hdnProcessorAct').value + ']');

    for (var arr in hdnAct) {
        procActData.push(hdnAct[arr]);
    }

    for (var arr in hdnData) {
        procActData.push(hdnData[arr]);
    }
    var data = google.visualization.arrayToDataTable(procActData);

    var options = {
        title: 'Daily utilization',
        vAxis: { title: 'Total Hours' },
        hAxis: { title: 'Utilization by Activity' },
        seriesType: 'bars',
        series: { 0: { type: 'line'} }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_div_pAct'));
    chart.draw(data, options);
}



function excolRevChart(obj) {
    var screenHt = screen.availHeight, screenWidth = screen.availWidth;
    var normalHt, normalWidth;
    var constHundred = 120;
    normalHt = normalWidth = "0px";

    //if clicked when it is minimized.
    if ($(obj).hasClass("popout")) {
        $("#reviewerContainer").removeClass("shorten").addClass("enlarge").css("height", screenHt);
        //$(obj).children(0).removeClass("glyphicon-resize-full").addClass("glyphicon-resize-small");
        $(obj).removeClass("popout").addClass("popin");
        $(obj).parent().css("margin-left", "0px");
        $("#chart_div_r").css({ "height": screenHt - constHundred, "width": screenWidth / 2 });
        $("#chart_div_rAct").css({ "height": screenHt - constHundred, "width": screenWidth / 2 });
        drawReviewerCharts(); //re-drawing the charts to fill the height and width.
    }
    else {
        $("#reviewerContainer").removeClass("enlarge").addClass("shorten");
        //$(obj).children(0).removeClass("glyphicon-resize-small").addClass("glyphicon-resize-full");
        $(obj).removeClass("popin").addClass("popout");
        $("#chart_div_r").css({ "height": normalHt, "width": normalWidth });
        $("#chart_div_rAct").css({ "height": normalHt, "width": normalWidth });
    }
}

function drawReviewerCharts() {
    drawReviewer();
    drawReviewerByAct();
}

function drawReviewer() {
    if (document.getElementById('hdnRev').value == "") return;
    var procData = [['Actul Utilization', 'Target Hrs', 'Total Time Spent']];
    //from hidden fields we will get array or arrays as data so we need to push each array to the above
    var hdnData = eval('[' + document.getElementById('hdnRev').value + ']');
    for (var arr in hdnData) {
        procData.push(hdnData[arr]);
    }
    // Some raw data (not necessarily accurate)

    var data = google.visualization.arrayToDataTable(procData);

    var options = {
        title: 'Daily utilization',
        vAxis: { title: 'Total Hours' },
        hAxis: { title: 'Actual Utilization' },
        seriesType: 'bars',
        series: { 0: { type: 'line'} }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_div_r'));
    chart.draw(data, options);
}

//for fullsize chart example see https://stackoverflow.com/questions/20764157/zoom-google-line-chart/36596155
function drawReviewerByAct() {
    if (document.getElementById('hdnRevAct').value == "") return;

    var procActData = [];
    var hdnAct = eval('[' + document.getElementById('hdnRevActivities').value + ']');
    var hdnData = eval('[' + document.getElementById('hdnRevAct').value + ']');

    for (var arr in hdnAct) {
        procActData.push(hdnAct[arr]);
    }

    for (var arr in hdnData) {
        procActData.push(hdnData[arr]);
    }
    var data = google.visualization.arrayToDataTable(procActData);

    var options = {
        title: 'Daily utilization',
        vAxis: { title: 'Total Hours' },
        hAxis: { title: 'Utilization by Activity' },
        seriesType: 'bars',
        series: { 0: { type: 'line'} }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_div_rAct'));
    chart.draw(data, options);
}