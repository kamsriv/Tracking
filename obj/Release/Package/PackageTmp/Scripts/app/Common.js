$(document).ready(function () {
    var pageSizes = [5, 10, 20, 50];
    var orderCol = 2;

    if (document.getElementById("hdnPager") != null)//Getting pager information from the user screen from a hidden variable.
    { pageSizes = eval(document.getElementById("hdnPager").value); }
    if (document.getElementById("hdnOrderCol") != null) { //Getting order by column if available in the screen
        orderCol = parseInt(document.getElementById("hdnOrderCol").value);
    }
    //Call a function that takes some argument and generate the table..
    $("table[for_datatbl='true']").each(function (cnt) {
        var tblId = $(this).attr('id');
        generateDataTable(tblId, pageSizes, orderCol);
    });
});
function generateDataTable(tblId, pageSizes, orderCol) {
    var table = $('#' + tblId).DataTable({
        //dom: 'Bfrtip' Rlfrtlip,          
        sDom: 'Bfrtlip',
        'order': [[orderCol, 'asc']], //setting the order of the column to description columns.
        'columnDefs': [{ 'targets': 'nosort', 'orderable': false }, { 'targets': 'novisible', 'visible': false}], //https://datatables.net/reference/option/columnDefs.targets
        buttons: ['copy', 'excel', 'pdf', 'print']
        , lengthMenu: pageSizes
    });
    $('#' + tblId + '_info').addClass('pull-left text-primary');
    $('#' + tblId + '_length').addClass('pull-left');
    $('#' + tblId + '_filter').addClass('pull-right');


    $('#select_all_existent').change(function () {
        var cells = table.cells().nodes();
        var enableSubBtn = false;
        //Applying selected class to all the selected rows if checked.
        if ($(this).is(':checked')) {
            $(table.rows().nodes()).addClass('info');

            //If not necessary remove below two lines
            $(table.rows().nodes()).each(function (i) {
                arrStatus.push($.trim($(this).find('td:eq(5)').attr("data-enable")));
            });
            //if not necessary
        }
        else {
            $(table.rows().nodes()).removeClass('info');

            //if not necessary
            arrStatus = [];
        }
        $(cells).find(':checkbox').prop('checked', $(this).is(':checked'));

        //if not necessary remove if condition.
        if (enableSubBtn > 0) {
            $("#btn_Sumbit").removeAttr("disabled");
        }

    });
}