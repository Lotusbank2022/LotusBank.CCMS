function showmsg(title, msg, msgtype) {

    swal({
        title: title,
        text: msg,
        type: msgtype
    });
}

function showloader() {
    if ($('#loaderpopup').css('display') == 'none') {
        $('#loaderbtn').click();
    }
}

function validateallinputs() {
    var valid = true;
    $(".form-control").each(function () {
        var classname = '';
        classname = $("#" + this.id).attr('class').toLocaleString().toString() + '';

        if (classname == 'form-control' || classname == 'form-control error') {
            if ($(this).val() == '') {
                $("#" + this.id).addClass('form-control error');
                valid = false;
            }
        }
    });
    return valid;
}