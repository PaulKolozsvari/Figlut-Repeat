function createDialog(dialogName, title, onOKCallBack, onOpenDialog, onCancelCallBack, dialogHeight, dialogWidth) {
    var buttons = undefined;
    if (onOKCallBack != undefined && onCancelCallBack != undefined) {
        buttons = [
            {
                text: "OK",
                class: 'ok-dialog-button',
                click: function () {
                    onOKCallBack();
                }
            },
            {
                text: "Cancel",
                class: 'cancel-dialog-button',
                click: function () {
                    onCancelCallBack();
                    $(this).dialog("close");
                    //$(this).remove();
                }
            }
        ];
    }
    else if (onOKCallBack == undefined && onCancelCallBack != undefined) {
        buttons = [
            {
                text: "Cancel",
                class: 'cancel-dialog-button',
                click: function () {
                    onCancelCallBack();
                    $(this).dialog("close");
                    //$(this).remove();
                }
            }
        ];
    }
    else if (onOKCallBack != undefined && onCancelCallBack == undefined) {
        buttons = [
            {
                text: "OK",
                class: 'ok-dialog-button',
                click: function () {
                    onOKCallBack();
                }
            },
        ];
    }
    else if (onOKCallBack == undefined && onCancelCallBack == undefined) {
    }

    $(dialogName).dialog({
        modal: true,
        autoOpen: false,
        show: {
            effect: "explode",
            duration: 600
        },
        hide: {
            effect: "puff",
            duration: 500
        },
        dialogClass: 'main-dialog-class',
        resizable: false,
        height: dialogHeight,
        width: dialogWidth,
        draggable: true,
        title: title,
        //buttons: {
        //    OK: function () {
        //        onOKCallBack();
        //    },
        //    Cancel: function () {
        //        onCancelCallBack();
        //        $(this).dialog("close");
        //    }
        //},
        buttons: buttons,
        open: function () {
            $(this).blur(); //remove focus from the close button.
            hookDialogOnEnterKeyPress(dialogName);
            onOpenDialog();
        },
        close: function (event, ui) {
            unhookDialogOnEnterKeyPress(dialogName);
        },
    });
    //}).prev(".ui-dialog-titlebar").css("color", "#3195c2");
    $(dialogName).parent().children().children('.ui-dialog-titlebar-close').hide();
}

function hookDialogOnEnterKeyPress(dialogName) {
    $(this).parent().find("button:eq(1)").focus();
    $(dialogName).keydown(function (event) {
        if (event.keyCode == 13) {
            $(this).parent().find("button:eq(1)").trigger("click");
            return false;
        }
    });
}

function unhookDialogOnEnterKeyPress(dialogName) {
    $(dialogName).unbind('keydown');
}