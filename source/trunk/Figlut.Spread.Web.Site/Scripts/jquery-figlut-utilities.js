/*
http://www.asp.net/mvc/overview/older-versions/using-the-html5-and-jquery-ui-datepicker-popup-calendar-with-aspnet-mvc/using-the-html5-and-jquery-ui-datepicker-popup-calendar-with-aspnet-mvc-part-4
Because new versions of browsers are implementing HTML5 incrementally, a good approach for now is to add code to your website that accommodates a wide variety of HTML5 support. For example, a more robust DatePickerReady.js script is shown below that lets your site support browsers that only partially support the HTML5 date control.
This script selects HTML5 input elements of type date that don't fully support the HTML5 date control. For those elements, it hooks up the jQuery UI popup calendar and then changes the type attribute from date to text. By changing the type attribute from date to text, partial HTML5 date support is eliminated.

*/
//if (!Modernizr.inputtypes.date) {
//    $(function () {
//        alert('test');
//        $("input[type='date']").datepicker().attr("type", "text");
//    })
//}

/*This function is to called by Ajax partial form postbacks.*/
function enableDatepickers() {
    //if (!Modernizr.inputtypes.date) { 
    //    //If it's an HTML 5 compatible browser, then don't use the jQuery datepicker, just rely on the native datepicker.
    //    $("input[type='date']").datepicker({ dateFormat: 'yy-mm-dd', inline: true }).attr("type", "text")
    //}
    $("input[type='date']").datepicker({ dateFormat: 'yy-mm-dd', inline: true }).attr("type", "text");
}

function enableDatepickerOnInputField(inputField) {
    var test = $('#' + inputField);
    if ($('#' + inputField).length > 0) {
        $('#' + inputField).datepicker({ dateFormat: 'yy-mm-dd', inline: true }).attr("type", "text");
        var test = $('#' + inputField);
        alert('date picker set: ' + inputField);
    }
    return;
    try {
        //$('#' + inputField).datepicker("destroy");
        //$('#' + inputField).removeClass('hasDatepicker').datepicker();
        test = $('#' + inputField);
        $('#' + inputField).removeClass('hasDatepicker').datepicker({ dateFormat: 'yy-mm-dd', inline: true }).attr("type", "text");
    }
    catch (error) {
        alert(error);
    }
}

/*This function is to called by Ajax partial form postbacks on forms showing grids.*/
function makeGridRowsPostForm() {
    /*Post the form when clicking the link.*/
    $('#gridTableContainer a').click(function () {
        var form = $('#frmDetails');
        form.attr("action", this.href);
        $(this).attr("href", "javascript:");
        form.submit();
        return false;
    });

    //Post the form when clicking a row.
    $('tbody tr').on('click', function () {
        //var value = $(this).closest('tr').find('td:eq(0)');
        var url = $(this).closest('tr').find('a').attr('href');
        if (url == undefined) {
            return;
        }
        var form = $('#frmDetails');
        form.attr("action", url);
        //$(this).attr("href", "javascript:");
        form.submit();
        return false;
    });
}

