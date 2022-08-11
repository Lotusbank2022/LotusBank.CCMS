
var selectedid = "";
var previousslide = "customerlistdiv";
var currentslide = "customerlistdiv";
var userid = '';
var clickdismissoverlay = false;
var userid = "";
var userpassword = "";
var signedin = '';

window.onload = function () {  };

window.onresize = function () {
    //set title to appear center of screen
    $('.title').css('left', "0");
}

function documentload() {

    //get states
    FillList('customer_address_state', getStates());
    FillList('customer_stateoforigin', getStates());



    signedin = localStorage.getItem('signedin')
    userid = localStorage.getItem('userid');
    userpassword = localStorage.getItem('userpassword')

    if (signedin == null || signedin == "") {
        if (userid != '' && userid != null && userpassword != '' && userpassword != null) {
            //window.location = 'index_offline.html';
        }
        //else
            //window.location = 'index.html';
    }

    userid = localStorage.getItem('userid');

    try {

        var photo = localStorage.getItem('photo');
        var signature = localStorage.getItem('signature');

        selectedid = localStorage.getItem('selectedid');

        if (selectedid != "" && selectedid != null || (localStorage.getItem('lastpage').toString() == 'signature' || localStorage.getItem('lastpage').toString() == 'camera'))
            editform(selectedid);

        if ((photo != "" && photo != null)) {
            document.getElementById('photo').src = photo;
        }

        if ((signature != "" && signature != null)) {
            document.getElementById('signature').src = signature;
        }
    } catch (ex) { }



    getaccounts();

    localStorage.setItem('lastpage', '');


    //set title to appear center of screen
    $('.title').css('left', "0");


    var sCacheStatus = "Not supported";
    if (window.applicationCache) {
        var oAppCache = window.applicationCache;
        switch (oAppCache.status) {
            case oAppCache.UNCACHED:
                sCacheStatus = "Not cached";
                break;
            case oAppCache.IDLE:
                sCacheStatus = "Idle";
                break;
            case oAppCache.CHECKING:
                sCacheStatus = "Checking";
                break;
            case oAppCache.DOWNLOADING:
                sCacheStatus = "Downloading";
                break;
            case oAppCache.UPDATEREADY:
                sCacheStatus = "Update ready";
                break;
            case oAppCache.OBSOLETE:
                sCacheStatus = "Obsolete";
                break;
            default:
                sCacheStatus = "Unexpected Status ( " +
                               oAppCache.status.toString() + ")";
                break;
        }
    }
    //alert(sCacheStatus);

    //if (window.applicationCache) {
    //    var appCache = window.applicationCache;
    //    appCache.addEventListener('error', appCacheError, false);
    //    appCache.addEventListener('checking', checkingEvent, false);
    //    appCache.addEventListener('noupdate', noUpdateEvent, false);
    //    appCache.addEventListener('downloading', downloadingEvent, false);
    //    appCache.addEventListener('progress', progressEvent, false);
    //    appCache.addEventListener('updateready', updateReadyEvent, false);
    //    appCache.addEventListener('cached', cachedEvent, false);
    //}
};


function appCacheError(error) {
    alert('appCacheError');
    alert(error);
}

function checkingEvent() {
    alert('checkingEvent');
}

function noUpdateEvent() {
    alert('noUpdateEvent');
}

function downloadingEvent() {
    alert('downloadingEvent');
}

function progressEvent() {
    //alert('progressEvent');
}

function updateReadyEvent() {
    alert('updateReadyEvent');
}

function cachedEvent() {
    alert('cachedEvent');
}

function logout() {
    localStorage.setItem('signedin', "");

    if (userid != '' && userid != null && userpassword != '' && userpassword != null) {
        window.location = 'index_offline.html';
    }
    else
        window.location = 'index.html';
}

function getStates() {
    var mydata = data;
    var state = [];
    var i;
    for (i = 0; i < mydata.length; ++i) {

        try {
            if (state.indexOf(mydata[i].state) == -1) {
                state.push(mydata[i].state);
            }
        } catch (ex) { state.push(mydata[i].state); }
    }

    return state;
}

function getLGA(state) {
    var mydata = data;
    var lga = [];
    var i;
    for (i = 0; i < mydata.length; ++i) {

        try {
            if (mydata[i].state == state) {
                lga.push(mydata[i].lga);
            }
        } catch (ex) { lga.push(mydata[i].lga); }
    }

    return lga;
}

function FillList(listname, list) {

    var selectbox = document.getElementById(listname);
    var i;
    for (i = selectbox.options.length - 1 ; i >= 0 ; i--) {
        selectbox.remove(i);
    }

    var opt = document.createElement('option');
    opt.value = '';
    opt.innerHTML = '';
    selectbox.appendChild(opt);


    for (i = 0; i < list.length; ++i) {
        var opt = document.createElement('option');
        opt.value = list[i];
        opt.innerHTML = list[i];
        selectbox.appendChild(opt);
    }


}

function CustomerAddressState_Change() {
    FillList('customer_address_lga', getLGA(document.getElementById('customer_address_state').value));
}

function CustomerStateOfOrigin_Change() {
    FillList('customer_stateoforiginlga', getLGA(document.getElementById('customer_stateoforigin').value));
}

function clearselectedid() {
    localStorage.setItem('selectedid', '');
}

function getaccounts() {

    var html = '';

    try {
        var customers = JSON.parse(window.localStorage.getItem("customers"));
        if (customers.length > 0) {
            for (var i = 0; i < customers.length; ++i) {

                var obj = customers[i];

                if (document.getElementById('searchaccounttxt').value != '') {

                    var customer_lastname = obj['customer_lastname'] + '';
                    var customer_firstname = obj['customer_firstname'] + '';
                    var customer_phone1 = obj['customer_phone1'] + '';
                    if (customer_lastname.toLocaleLowerCase().indexOf(document.getElementById('searchaccounttxt').value.toLocaleLowerCase()) == -1 && customer_firstname.toLocaleLowerCase().indexOf(document.getElementById('searchaccounttxt').value.toLocaleLowerCase()) == -1 && customer_phone1.toLocaleLowerCase().indexOf(document.getElementById('searchaccounttxt').value.toLocaleLowerCase()) == -1) {
                        continue;
                    }
                }

                var photo = obj['photo'];

                if (document.getElementById('delete_cancel_menu').style.display == 'inline') {
                    html += "<div class=\"row no-gap\"> ";

                    html += "<div class=\"col-10\">";
                    html += " <label id=deletechk_" + obj['recordid'] + "  class=\"item-checkbox item-content\" style=\"padding-left:0px; padding-top:30px;\">";
                    html += "<input type=\"checkbox\" id=\"customer_chk" + obj['recordid'] + "\" > <i class=\"icon icon-checkbox\"   style=\"margin-right:15px;  margin-top:25px;  margin-left:5px;\"></i> </label>";
                    html += "</div>";


                    html += "<div class=\"col-90\"> ";
                    html += "<li>  <a href=\"#\" class=\"item-link item-content\"> ";
                    html += "<div class=\"item-media\"><img id=\"customerimg_" + obj['recordid'] + "\" onclick=\"showphoto(this.src,'customerlistdiv')\" src=\"" + photo + "\" style=\"border-radius:5px; cursor:pointer; width:100px; height:100px; border-style:ridge; border-width:1px; border-color:#e9e9e9; \" alt=\"passport photo\" />";
                    html += "</div><div  onclick=\"editform('" + obj['recordid'] + "'); \" class=\"item-inner\"><div class=\"item-title\" style=\"margin-top:10px\"><b>" + obj['customer_lastname'] + " " + obj['customer_firstname'] + "</b>";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_email'] + "</div>  ";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_phone1'] + "</div>  ";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_dob'] + "</div>";
                    html += "</div> <div id=\"customereditbtn_" + obj['recordid'] + "\" class=\"item-after\">Edit</div> </div> </a></li>";
                    html += "</div>";

                    html += "</div>";
                }
                else {

                    var photo = obj['photo'];
                    html += "<li>  <a href=\"#\" class=\"item-link item-content\"> <div class=\"item-media\"> <label id=deletechk_" + obj['recordid'] + " style=\"display:none;\"  class=\"item-checkbox item-content\" style=\"padding-left:0px;\">";
                    html += "<input type=\"checkbox\" id=\"customer_chk" + obj['recordid'] + "\" > <i class=\"icon icon-checkbox\"   style=\"margin-right:15px; margin-top:15px;\"></i> </label>";
                    html += "<img id=\"customerimg_" + obj['recordid'] + "\" onclick=\"showphoto(this.src,'customerlistdiv')\" src=\"" + photo + "\" style=\"border-radius:5px; cursor:pointer; width:100px; height:100px; border-style:ridge; border-width:1px; border-color:#e9e9e9; \" alt=\"passport photo\" />";
                    html += "</div><div  onclick=\"editform('" + obj['recordid'] + "'); \" class=\"item-inner\"><div class=\"item-title\" style=\"margin-top:10px\"><b>" + obj['customer_lastname'] + " " + obj['customer_firstname'] + "</b>";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_email'] + "</div>  ";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_phone1'] + "</div>  ";
                    html += "<div style=\"padding-top:5px; margin-bottom:10px\" class=\"item-header\">" + obj['customer_dob'] + "</div>";
                    html += "</div> <div id=\"customereditbtn_" + obj['recordid'] + "\" class=\"item-after\">Edit</div> </div> </a></li>";
                }
            }
            html = "<ul>" + html + "</ul>";
        }
        else
            html = '<div style="\padding-top:20px; padding-left:20px; padding-bottom:50px;\">NO RECORD TO DISPLAY<div>';
    } catch (err) { html = '<div style="\padding-top:20px; padding-left:20px; padding-bottom:50px;\">NO RECORD TO DISPLAY<div>'; }

    document.getElementById('customerlistdiv').innerHTML = html;

    hidemodalbg();
}

function deleteaccounts() {
    if (document.getElementById('confirmationpasswordtxt').value != localStorage.getItem('userpassword').toString()) {
        showmodalbg(true);
        hidedeleteaccountconfirmation();
        errorbox('Invalid Password', '');
        return false;
    }

    //delete selected records
    var customers = JSON.parse(window.localStorage.getItem("customers"));

    if (customers.length > 0) {
        for (var i = 0; i <= customers.length; ++i) {
            try {
                var record = customers[i];
                if (document.getElementById('customer_chk' + record.recordid).checked == true) {
                    customers.splice(i, 1);
                    i -= 1;
                }
            } catch (ex) { }
        }
    }

    customers = generaterecordid(customers);
    window.localStorage.setItem('customers', JSON.stringify(customers));

    getaccounts();


    hidedeleteaccountconfirmation();
    hidedeletecancelmenu();
    showverticalfillmenu();
    msgbox('Record Deleted');
}

function showphoto(path, dismispage) {

    previousslide = dismispage;
    document.getElementById('modalphotoimg').src = path;
    $('#' + previousslide).hide();
    $('#footernavbar').hide();
    $('.navbar').hide();
    $('#photodiv').show();

    hidesearcharea();
}

function hidephoto() {
    $('#' + previousslide).show();
    $('#photodiv').hide();
    $('#footernavbar').show();
    $('.navbar').show();

    if (previousslide == 'customerlistdiv')
        showsearcharea();
}

function showslide(slide, animate) {
    if (slide != currentslide) {
        $('#' + slide).show();

        if (animate)
            $('#' + currentslide).slideUp('fast');
        else
            $('#' + currentslide).hide();

        previousslide = currentslide;
        currentslide = slide;
    }
}

function showmodalbg(option) {
    clickdismissoverlay = option;

    if (document.getElementById('searcharea').style.display == 'none') {
        document.getElementById('modalbg').style.top = '65px';
    }
    else {
        document.getElementById('modalbg').style.top = '115px';
    }
    document.getElementById('modalbg').style.display = 'inline';
}

function hidemodalbg() {
    document.getElementById('modalbg').style.display = 'none';
}

function modalbg_click() {
    if (clickdismissoverlay) {
        hidemodalbg();
        hidepopupmenu();
    }
}

function hidesearcharea() {
    $('#searcharea').hide();
}

function showsearcharea() {
    $('#searcharea').show();
}

function hidefooternavbar() {
    document.getElementById('footernavbar').style.display = 'none';
}

function showfooternavbar() {
    document.getElementById('footernavbar').style.display = 'inline';
}

function hidepopupmenu() {
    $('#popupmenu').hide();
    hidemodalbg();
}

function showpopupmenu() {
    $('#popupmenu').show();
    showmodalbg(true);
}

function togglepopupmenu() {
    if (document.getElementById('popupmenu').style.display == 'inline') {
        document.getElementById('popupmenu').style.display = 'none';
        hidemodalbg();
    }
    else {
        document.getElementById('popupmenu').style.display = 'inline';
        showmodalbg(true);
    }

}

function opendeletecheckbox() {
    //var customers = JSON.parse(window.localStorage.getItem("customers"));
    //if (customers.length > 0) {
    //    for (var i = 0; i < customers.length; ++i) {
    //        document.getElementById('deletechk_' + i).style.display = 'inline';
    //        document.getElementById('customer_chk' + i).checked = false;
    //        document.getElementById('customereditbtn_' + i).style.display = 'none';
    //    }
    //}
    hidemodalbg();
    hidepopupmenu();
    showdeletecancelmenu();
    getaccounts();
}

function closedeletecheckbox() {

    //var customers = JSON.parse(window.localStorage.getItem("customers"));
    //if (customers.length > 0) {
    //    for (var i = 0; i < customers.length; ++i) {
    //        document.getElementById('deletechk_' + i).style.display = 'none';
    //        //document.getElementById('customereditbtn_' + i).style.display = 'inline';
    //    }
    //}
    hidemodalbg();
    hidepopupmenu();

}

function showdeletecancelmenu() {
    hideverticalfillmenu();
    showdeletecancelmenu();
}

function showdeletecancelmenu() {
    document.getElementById('delete_cancel_menu').style.display = 'inline';
}

function hidedeletecancelmenu() {
    document.getElementById('delete_cancel_menu').style.display = 'none';
    getaccounts();
}

function hideverticalfillmenu() {
    document.getElementById('vertical_fill_menu').style.display = 'none';
}

function showverticalfillmenu() {
    document.getElementById('vertical_fill_menu').style.display = 'inline';
}

function selectphoto() {
    document.getElementById('photofile').click();
}

function selectedphoto() {
    var reader = new FileReader();
    reader.onload = function (e) {

         if ((document.getElementById('photofile').files[0].size/1024) <= 500) {
            $('#photo').attr('src', e.target.result);
            localStorage.setItem('photo', e.target.result);
        }
        else {
            resizeBase64Img(e.target.result).then(function (src) {
                $('#photo').attr('src', src);
                localStorage.setItem('photo', src);
            });
        }
    }
    reader.readAsDataURL(document.getElementById('photofile').files[0]);
}

function cameracapture(caller) {

    if (document.URL.indexOf('http://') != -1 || document.URL.indexOf('https://') != -1) {
        errorbox("FEATURE ONLY AVAILABLE ON MOBILE APP", "");
        return false;
        $("#photocamerabtn").hide();
        $("#signaturecamerabtn").hide();
    }

    localStorage.setItem('cameracaller', caller);
    localStorage.setItem('selectedid', selectedid);
    window.location = 'camera.html';
}

function selectsignature() {
    document.getElementById('signaturefile').click();
}

function selectedsignature() {
    var reader = new FileReader();
    reader.onload = function (e) {

        if ((document.getElementById('signaturefile').files[0].size / 1024) <= 500) {
            $('#signature').attr('src', e.target.result);
            localStorage.setItem('signature', e.target.result);
        }
        else {
            resizeBase64Img(e.target.result).then(function (src) {
                $('#signature').attr('src', src);
                localStorage.setItem('signature', src);
            });
        }
    }
    reader.readAsDataURL(document.getElementById('signaturefile').files[0]);
}

function savesettings() {

    if (document.getElementById('currentpassword').value == '') {
        errorbox('ENTER CURRENT PASSWORD', '');
        return false;
    }

    if (document.getElementById('newpassword').value == '') {
        errorbox('ENTER NEW PASSWORD', '');
        return false;
    }

    if (document.getElementById('confirmnewpassword').value == '') {
        errorbox('CONFIRM NEW PASSWORD', '');
        return false;
    }

    if (document.getElementById('newpassword').value != document.getElementById('confirmnewpassword').value) {
        errorbox('PASSWORD NOT CONSISTENT', '');
        return false;
    }


    if (document.getElementById('currentpassword').value != window.localStorage.getItem('userpassword')) {
        errorbox('CURRENT PASSWORD IS INVALID', '');
        return false;
    }

    if (document.getElementById('currentpassword').value == document.getElementById('newpassword').value) {
        errorbox('NEW PASSWORD INVALID', '');
        return false;
    }

    window.localStorage.setItem('userpassword', document.getElementById('newpassword').value);

    document.getElementById('currentpassword').value = document.getElementById('confirmnewpassword').value = document.getElementById('newpassword').value = document.getElementById('confirmnewpassword').value = '';

    msgbox('PASSWORD UPDATED', '');

    showaccounts();
}

function clearform() {
    document.getElementById('customer_title').value = '';
    document.getElementById('customer_lastname').value = '';
    document.getElementById('customer_firstname').value = '';
    document.getElementById('customer_othernames').value = '';
    document.getElementById('customer_maritalstatus').value = '';
    document.getElementById('customer_gender').value = '';
    document.getElementById('customer_placeofbirth').value = '';
    document.getElementById('customer_dob').value = '';
    document.getElementById('customer_mothermaidenname').value = '';
    document.getElementById('customer_stateoforigin').value = '';
    document.getElementById('customer_stateoforiginlga').value = '';
    document.getElementById('customer_bvn').value = '';
    document.getElementById('customer_streetnumber').value = '';
    document.getElementById('customer_streetname').value = '';
    document.getElementById('customer_address_state').value = '';
    document.getElementById('customer_address_lga').value = '';
    document.getElementById('customer_phone1').value = '';
    document.getElementById('customer_phone2').value = '';
    document.getElementById('customer_email').value = '';
    document.getElementById('customer_accounttype').value = '';
    document.getElementById('customer_branch').value = '';
    document.getElementById('customer_cardtype').value = '';
    document.getElementById('customer_alerttype').value = '';

    document.getElementById('banking_internet').checked = false;
    document.getElementById('banking_mobile').checked = false;
    document.getElementById('banking_atm_pos').checked = false;
    document.getElementById('banking_other').checked = false;


    document.getElementById('customer_nextofkinsurname').value = '';
    document.getElementById('customer_nextofkinfirstname').value = '';

    document.getElementById('photo').src = 'resources/img/defaultuserimage.png'
    document.getElementById('signature').src = 'resources/img/signature.png'
    localStorage.setItem('photo', document.getElementById('photo').src);
    localStorage.setItem('signature', document.getElementById('signature').src);
    localStorage.setItem('selectedid', "");
    selectedid = "";
}

function getSelectedText(elt) {
    if (elt.selectedIndex == -1)
        return '';

    return elt.options[elt.selectedIndex].text;
}

function saveform() {
    var inputevalidated = true

    var photopath = document.getElementById('photo').src.toString() + "";
    var signaturepath = document.getElementById('signature').src.toString() + "";


    if (photopath.indexOf("resources/img/defaultuserimage.png") != -1) {
        errorbox('PHOTO IS MISSING');
        inputevalidated = false;
        return false;
    }

    if (signaturepath.indexOf("resources/img/signature.png") != -1) {
        errorbox('SIGNATURE IS MISSING');
        inputevalidated = false;
        return false;
    }


    //ensure all fields are filled
    $('.inputfield').each(function (index, elem) {
        if (elem.value.toString() == '') {
            errorbox('Form is incomplete!', 'long');
            elem.focus();
            inputevalidated = false;
            return false;
        }
    });


    if (inputevalidated == false)
        return false;

    if (document.getElementById('customer_bvn').value.length != 11) {
        errorbox('BVN MUST BE 11 DIGITS');
        document.getElementById('customer_bvn').focus();
        return false;
    }

    if (document.getElementById('customer_phone1').value.length != 11) {
        errorbox('PHONE NUMBER MUST BE 11 DIGITS');
        document.getElementById('customer_phone1').focus();
        return false;
    }

    if (Date.parse(document.getElementById('customer_dob').value) >= Date.now()) {
        errorbox('INVALID DATE OF BIRTH');
        document.getElementById('customer_dob').focus();
        return false;
    }



    var customers = [];
    try {
        customers = JSON.parse(window.localStorage.getItem("customers"));
        if (customers === null || customers == '')
            customers = [];
    } catch (ex) { customers = []; }

    var photo = document.getElementById('photo').src;
    var signature = document.getElementById('signature').src;


    if (selectedid === '' || selectedid == '' || selectedid == "" || selectedid == null) {
        var d = new Date();
        var refid = d.getDay() + '' + d.getMonth() + '' + d.getFullYear() + '' + d.getHours() + '' + d.getMinutes() + '' + d.getSeconds() + '' + d.getMilliseconds();


        var obj = {
            'recordid': '',
            'ref_id': refid,
            'customer_title': document.getElementById('customer_title').value,
            'customer_lastname': document.getElementById('customer_lastname').value,
            'customer_firstname': document.getElementById('customer_firstname').value,
            'customer_othernames': document.getElementById('customer_othernames').value,
            'customer_maritalstatus': document.getElementById('customer_maritalstatus').value,
            'customer_gender': document.getElementById('customer_gender').value,
            'customer_placeofbirth': document.getElementById('customer_placeofbirth').value,
            'customer_dob': document.getElementById('customer_dob').value,
            'customer_mothermaidenname': document.getElementById('customer_mothermaidenname').value,
            'customer_stateoforigin': getSelectedText(document.getElementById('customer_stateoforigin')),
            'customer_stateoforiginlga': getSelectedText(document.getElementById('customer_stateoforiginlga')),
            'customer_bvn': document.getElementById('customer_bvn').value,
            'customer_streetnumber': document.getElementById('customer_streetnumber').value,
            'customer_streetname': document.getElementById('customer_streetname').value,
            'customer_address_state': getSelectedText(document.getElementById('customer_address_state')),
            'customer_address_lga': getSelectedText(document.getElementById('customer_address_lga')),
            'customer_phone1': document.getElementById('customer_phone1').value,
            'customer_phone2': document.getElementById('customer_phone2').value,
            'customer_email': document.getElementById('customer_email').value,
            'customer_accounttype': document.getElementById('customer_accounttype').value,
            'customer_branch': document.getElementById('customer_branch').value,
            'customer_cardtype': document.getElementById('customer_cardtype').value,
            'banking_internet': document.getElementById('banking_internet').checked,
            'banking_mobile': document.getElementById('banking_mobile').checked,
            'banking_atm_pos': document.getElementById('banking_atm_pos').checked,
            'banking_other': document.getElementById('banking_other').checked,
            'customer_alerttype': document.getElementById('customer_alerttype').value,
            'customer_nextofkinsurname': document.getElementById('customer_nextofkinsurname').value,
            'customer_nextofkinfirstname': document.getElementById('customer_nextofkinfirstname').value,
            'photo': photo, 'signature': signature
        };
        customers.push(obj);
    }
    else {

        for (var i = 0; i < customers.length; ++i) {
            var obj = customers[i];
            if (obj['recordid'] == selectedid) {
                obj['customer_title'] = document.getElementById('customer_title').value;
                obj['customer_lastname'] = document.getElementById('customer_lastname').value;
                obj['customer_firstname'] = document.getElementById('customer_firstname').value;
                obj['customer_othernames'] = document.getElementById('customer_othernames').value,
                obj['customer_maritalstatus'] = document.getElementById('customer_maritalstatus').value;
                obj['customer_gender'] = document.getElementById('customer_gender').value;
                obj['customer_placeofbirth'] = document.getElementById('customer_placeofbirth').value;
                obj['customer_dob'] = document.getElementById('customer_dob').value;
                obj['customer_mothermaidenname'] = document.getElementById('customer_mothermaidenname').value;
                obj['customer_stateoforigin'] = getSelectedText(document.getElementById('customer_stateoforigin'));
                obj['customer_stateoforiginlga'] = getSelectedText(document.getElementById('customer_stateoforiginlga'));
                obj['customer_bvn'] = document.getElementById('customer_bvn').value;
                obj['customer_streetnumber'] = document.getElementById('customer_streetnumber').value;
                obj['customer_streetname'] = document.getElementById('customer_streetname').value;
                obj['customer_address_state'] = getSelectedText(document.getElementById('customer_address_state'));
                obj['customer_address_lga'] = getSelectedText(document.getElementById('customer_address_lga'));
                obj['customer_phone1'] = document.getElementById('customer_phone1').value;
                obj['customer_phone2'] = document.getElementById('customer_phone2').value;
                obj['customer_email'] = document.getElementById('customer_email').value;
                obj['customer_accounttype'] = document.getElementById('customer_accounttype').value;
                obj['customer_branch'] = document.getElementById('customer_branch').value;
                obj['customer_cardtype'] = document.getElementById('customer_cardtype').value;
                obj['banking_internet'] = document.getElementById('banking_internet').checked;
                obj['banking_mobile'] = document.getElementById('banking_mobile').checked;
                obj['banking_atm_pos'] = document.getElementById('banking_atm_pos').checked;
                obj['banking_other'] = document.getElementById('banking_other').checked;
                obj['customer_alerttype'] = document.getElementById('customer_alerttype').value;
                obj['customer_nextofkinsurname'] = document.getElementById('customer_nextofkinsurname').value;
                obj['customer_nextofkinfirstname'] = document.getElementById('customer_nextofkinfirstname').value;

                obj['photo'] = photo;
                obj['signature'] = signature;
            }
        }
    }

    selectedid = "";
    customers = generaterecordid(customers);
    window.localStorage.setItem('customers', JSON.stringify(customers));
    msgbox('RECORD SAVED', 'short');
    clearform();
    getaccounts();

    setTimeout("showaccounts();", 1000);
}

function editform(id) {

    //hidedeletecancelmenu(); showverticalfillmenu(); closedeletecheckbox();
    if (document.getElementById('delete_cancel_menu').style.display == 'inline') {
        document.getElementById('deletechk_' + id).click();
        return false;
    }

    try {
        var customers = JSON.parse(window.localStorage.getItem("customers"));
        if (customers.length > 0) {
            for (var i = 0; i < customers.length; ++i) {
                var obj = customers[i];
                var cid = obj['recordid'] + "";
                if (cid == id) {
                    selectedid = id;
                    document.getElementById('customer_title').value = obj['customer_title'];
                    document.getElementById('customer_lastname').value = obj['customer_lastname'];
                    document.getElementById('customer_firstname').value = obj['customer_firstname'];
                    document.getElementById('customer_othernames').value = obj['customer_othernames'];
                    document.getElementById('customer_maritalstatus').value = obj['customer_maritalstatus'];
                    document.getElementById('customer_gender').value = obj['customer_gender'];
                    document.getElementById('customer_placeofbirth').value = obj['customer_placeofbirth'];
                    document.getElementById('customer_dob').value = obj['customer_dob'];
                    document.getElementById('customer_mothermaidenname').value = obj['customer_mothermaidenname'];
                    document.getElementById('customer_stateoforigin').value = obj['customer_stateoforigin'];

                    //get LGA's of selected state of origin
                    CustomerStateOfOrigin_Change();
                    document.getElementById('customer_stateoforiginlga').value = obj['customer_stateoforiginlga'];

                    document.getElementById('customer_bvn').value = obj['customer_bvn'];
                    document.getElementById('customer_streetnumber').value = obj['customer_streetnumber'];
                    document.getElementById('customer_streetname').value = obj['customer_streetname'];
                    document.getElementById('customer_address_state').value = obj['customer_address_state'];

                    //get LGA's of selected state of residence
                    CustomerAddressState_Change();

                    document.getElementById('customer_address_lga').value = obj['customer_address_lga'];
                    document.getElementById('customer_phone1').value = obj['customer_phone1'];
                    document.getElementById('customer_phone2').value = obj['customer_phone2'];
                    document.getElementById('customer_email').value = obj['customer_email'];

                    document.getElementById('customer_accounttype').value = obj['customer_accounttype'];
                    document.getElementById('customer_branch').value = obj['customer_branch'];
                    document.getElementById('customer_cardtype').value = obj['customer_cardtype'];

                    document.getElementById('banking_internet').checked = obj['banking_internet'];
                    document.getElementById('banking_mobile').checked = obj['banking_mobile'];
                    document.getElementById('banking_atm_pos').checked = obj['banking_atm_pos'];
                    document.getElementById('banking_other').checked = obj['banking_other'];

                    document.getElementById('customer_alerttype').value = obj['customer_alerttype'];
                    document.getElementById('customer_nextofkinsurname').value = obj['customer_nextofkinsurname'];
                    document.getElementById('customer_nextofkinfirstname').value = obj['customer_nextofkinfirstname'];


                    document.getElementById('photo').src = obj['photo'];
                    document.getElementById('signature').src = obj['signature'];
                    localStorage.setItem('photo', document.getElementById('photo').src);
                    localStorage.setItem('signature', document.getElementById('signature').src);
                    localStorage.setItem('selectedid', selectedid);
                    window.scrollTo(0, 0);

                    break;
                }
            }
        }
    } catch (err) { }

    showform();
}

function generaterecordid(list) {

    try {
        if (list.length > 0) {
            for (var i = 0; i < list.length; ++i) {
                var obj = list[i];
                obj['recordid'] = i;
            }
        }
    } catch (err) { }

    return list;
}

function msgbox(msg, duration) {
    $('.message').html('<span style=\"text-align:center; width:100%;\">' + msg + '</span>').fadeIn(400).delay(1000).fadeOut(400);
}

function errorbox(msg, duration) {
    $('.error').html('<span style=\"text-align:center; width:100%;\">' + msg + '</span>').fadeIn(400).delay(1000).fadeOut(400);
}

function showconfirmdelete() {
    var checkcount = 0;
    var customers = JSON.parse(window.localStorage.getItem("customers"));
    if (customers.length > 0) {
        for (var i = 0; i < customers.length; ++i) {
            if (document.getElementById('customer_chk' + i).checked) {
                checkcount += 1;
                break;
            }
        }
    }

    if (checkcount == 0) {
        alert('No account selected');
        return false;
    }
    showmodalbg(false);
    showdeleteaccountconfirmation();
}

function showdeleteaccountconfirmation() {
    document.getElementById('deleteaccountconfirmation').style.display = 'inline';
}

function hidedeleteaccountconfirmation() {
    document.getElementById('deleteaccountconfirmation').style.display = 'none';
    hidemodalbg();
}

function showaccounts() {
    showsearcharea();
    $('#accountlistmenu').removeClass().addClass('tab-link tab-link-active');
    $('#formmenu').removeClass().addClass('tab-link');
    $('#uploadmenu').removeClass().addClass('tab-link');
    $('#settingsmenu').removeClass().addClass('tab-link');
    document.getElementById('footnavbarhighlight').style.left = '0%';

    $('#customerlistdiv').show();
    $('#uploaddiv').hide();
    $('#settingsdiv').hide();
    $('#signaturediv').hide();
    document.getElementById('formdiv').style.display = 'none';
    $('#deleteaccountbtn').show();
    $('#clearformbtn').hide();
    hidemodalbg();
    hidepopupmenu();
}

function showform() {
    $('#formmenu').removeClass().addClass('tab-link tab-link-active');
    $('#accountlistmenu').removeClass().addClass('tab-link');
    $('#uploadmenu').removeClass().addClass('tab-link');
    $('#settingsmenu').removeClass().addClass('tab-link');

    document.getElementById('footnavbarhighlight').style.left = '22%';
    document.getElementById('customerlistdiv').style.display = 'none';
    $('#uploaddiv').hide();
    $('#settingsdiv').hide();
    $('#signaturediv').hide();

    document.getElementById('formdiv').style.display = 'inline';

    hidesearcharea();

    $('#deleteaccountbtn').hide();
    $('#clearformbtn').show();
}

function showupload() {

    $('#uploadmenu').removeClass().addClass('tab-link tab-link-active');
    $('#accountlistmenu').removeClass().addClass('tab-link');
    $('#formmenu').removeClass().addClass('tab-link');
    $('#settingsmenu').removeClass().addClass('tab-link');
    document.getElementById('footnavbarhighlight').style.left = '46%';

    document.getElementById('customerlistdiv').style.display = 'none';
    $('#uploaddiv').show();
    document.getElementById('formdiv').style.display = 'none';
    $('#settingsdiv').hide();
    $('#signaturediv').hide();
    hidesearcharea();

    $('#deleteaccountbtn').hide();
    $('#clearformbtn').hide();


    var url = window.localStorage.getItem('defaultserverpath') + '/upload.aspx';

    $('#uploadstatusdiv').html('<pan style=\"color:black;\">PLEASE WAIT.... </pan>');


    var customers = JSON.parse(window.localStorage.getItem("customers"));
    if (customers.length == 0) {
        $('#uploadstatusdiv').html('<b style=\"color:red;\">NO RECORD TO UPLOAD! </b>');
        return false;
    }




    var url = window.localStorage.getItem('defaultserverpath') + '/index.html';
    $.ajax({
        type: "GET",
        url: url,
        data: {},
        timeout: 50000,
        success: function (result) {

            try {
                var customers = JSON.parse(window.localStorage.getItem("customers"));
                if (customers.length == 0) {
                    $('#uploadstatusdiv').html('<b style=\"color:red;\">NO RECORD TO UPLOAD! </b>');
                    return false;
                }

                uploadrecord(0);
            } catch (ex) {
                $('#uploadstatusdiv').html('<b style=\"color:red;\">NO RECORD TO UPLOAD! </b>');
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $('#uploadstatusdiv').html('<b style=\"color:red;\">NETWORK UNAVAILABLE! </b>');
        }
    });
}

function showsettings() {
    $('#settingsmenu').removeClass().addClass('tab-link tab-link-active');
    $('#accountlistmenu').removeClass().addClass('tab-link');
    $('#uploadmenu').removeClass().addClass('tab-link');
    $('#formmenu').removeClass().addClass('tab-link');


    document.getElementById('footnavbarhighlight').style.left = '75%';

    document.getElementById('customerlistdiv').style.display = 'none';
    $('#settingsdiv').show();
    $('#uploaddiv').hide();
    $('#signaturediv').hide();
    document.getElementById('formdiv').style.display = 'none';

    $('#deleteaccountbtn').hide();
    $('#clearformbtn').hide();

    hidesearcharea();
}

function SignPad() {
    $('#signaturediv').show();
    document.getElementById('formdiv').style.display = 'none';
    // window.location = 'signature/example/signaturepage.html';
}

function signaturedone() {
    var signature = localStorage.getItem('signature');
    if ((signature != "" && signature != null)) {
        document.getElementById('signature').src = signature;
    }

    showform();
}

function refrshupload(html) {
    $('#uploadstatusdiv').html(html);
}

function uploadrecord(index) {

    var customers = JSON.parse(window.localStorage.getItem("customers"));
    if ((index + 1) <= customers.length)
        $('#uploadstatusdiv').text('Uploading.. ' + (index + 1) + ' / ' + customers.length);


    if (customers.length > 0) {

        var ImageURL = customers[index]['photo'];
        var block = ImageURL.split(";");
        var contentType = block[0].split(":")[1];
        var realData = block[1].split(",")[1];
        var photoblob = b64toBlob(realData, contentType);

        ImageURL = customers[index]['signature']
        block = ImageURL.split(";");
        contentType = block[0].split(":")[1];
        realData = block[1].split(",")[1];
        var signatureblob = b64toBlob(realData, contentType);

        // Create a FormData and append the file
        var fd = new FormData();

        fd.append("ref_id", customers[index]['ref_id']);
        fd.append("customer_title", customers[index]['customer_title']);
        fd.append("customer_lastname", customers[index]['customer_lastname']);
        fd.append("customer_firstname", customers[index]['customer_firstname']);
        fd.append("customer_othernames", customers[index]['customer_othernames']);
        fd.append("customer_maritalstatus", customers[index]['customer_maritalstatus']);
        fd.append("customer_gender", customers[index]['customer_gender']);
        fd.append("customer_placeofbirth", customers[index]['customer_placeofbirth']);
        fd.append("customer_dob", customers[index]['customer_dob']);
        fd.append("customer_mothermaidenname", customers[index]['customer_mothermaidenname']);
        fd.append("customer_stateoforigin", customers[index]['customer_stateoforigin']);
        fd.append("customer_stateoforiginlga", customers[index]['customer_stateoforiginlga']);
        fd.append("customer_bvn", customers[index]['customer_bvn']);
        fd.append("customer_streetnumber", customers[index]['customer_streetnumber']);
        fd.append("customer_streetname", customers[index]['customer_streetname']);
        fd.append("customer_address_state", customers[index]['customer_address_state']);
        fd.append("customer_address_lga", customers[index]['customer_address_lga']);
        fd.append("customer_phone1", customers[index]['customer_phone1']);
        fd.append("customer_phone2", customers[index]['customer_phone2']);
        fd.append("customer_email", customers[index]['customer_email']);
        fd.append("customer_accounttype", customers[index]['customer_accounttype']);
        fd.append("customer_branch", customers[index]['customer_branch']);
        fd.append("customer_cardtype", customers[index]['customer_cardtype']);
        fd.append("customer_alerttype", customers[index]['customer_alerttype']);
        fd.append("banking_internet", customers[index]['banking_internet']);
        fd.append("banking_mobile", customers[index]['banking_mobile']);
        fd.append("banking_atm_pos", customers[index]['banking_atm_pos']);
        fd.append("banking_other", customers[index]['banking_other']);
        fd.append("customer_nextofkinsurname", customers[index]['customer_nextofkinsurname']);
        fd.append("customer_nextofkinfirstname", customers[index]['customer_nextofkinfirstname']);
        fd.append("photo", photoblob);
        fd.append("signature", signatureblob);
        fd.append("userid", userid);
        fd.append("userpassword", localStorage.getItem('userpassword'));


        $.ajax({
            type: "POST",
            url: localStorage.getItem('defaultserverpath') + "/accountupload.aspx",
            cache: false,
            contentType: false,
            processData: false,
            data: fd,
            //data: {
            //ref_id: customers[index]['ref_id'],
            //customer_title: customers[index]['customer_title'],
            //customer_lastname: customers[index]['customer_lastname'],
            //customer_firstname: customers[index]['customer_firstname'],
            //customer_othernames: customers[index]['customer_othernames'],
            //customer_maritalstatus: customers[index]['customer_maritalstatus'],
            //customer_gender: customers[index]['customer_gender'],
            //customer_placeofbirth: customers[index]['customer_placeofbirth'],
            //customer_dob: customers[index]['customer_dob'],
            //customer_mothermaidenname: customers[index]['customer_mothermaidenname'],
            //customer_stateoforigin: customers[index]['customer_stateoforigin'],
            //customer_stateoforiginlga: customers[index]['customer_stateoforiginlga'],
            //customer_bvn: customers[index]['customer_bvn'],
            //customer_streetnumber: customers[index]['customer_streetnumber'],
            //customer_streetname: customers[index]['customer_streetname'],
            //customer_address_state: customers[index]['customer_address_state'],
            //customer_address_lga: customers[index]['customer_address_lga'],
            //customer_phone1: customers[index]['customer_phone1'],
            //customer_phone2: customers[index]['customer_phone2'],
            //customer_email: customers[index]['customer_email'],
            //customer_accounttype: customers[index]['customer_accounttype'],
            //customer_branch: customers[index]['customer_branch'],
            //customer_cardtype: customers[index]['customer_cardtype'],
            //customer_alerttype: customers[index]['customer_alerttype'],
            //banking_internet: customers[index]['banking_internet'],
            //banking_mobile: customers[index]['banking_mobile'],
            //banking_atm_pos: customers[index]['banking_atm_pos'],
            //banking_other: customers[index]['banking_other'],
            //customer_nextofkinsurname: customers[index]['customer_nextofkinsurname'],
            //customer_nextofkinfirstname: customers[index]['customer_nextofkinfirstname'],
            //photo: customers[index]['photo'],
            //signature: customers[index]['signature'],
            //userid: userid,
            //userpassword: localStorage.getItem('userpassword')
            //},
            timeout: 60000,//six seconds
            success: function (result) {
                if (result == 'success') {
                    var customers = JSON.parse(window.localStorage.getItem("customers"));
                    customers.splice(0, 1);

                    //re-generate record id's
                    customers = generaterecordid(customers);

                    window.localStorage.setItem('customers', JSON.stringify(customers));
                    uploadrecord(0);
                }
                else {
                    var customers = JSON.parse(window.localStorage.getItem("customers"));
                    $('#uploadstatusdiv').html('<span style=\"color:red;\">Upload failed for: ' + customers[0].customer_lastname + ' ' + customers[0].customer_firstname + ' </span>');
                    logerrortoserver(result);
                    return false;
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                logerrortoserver(thrownError);
                $('#uploadstatusdiv').html('<b style=\"color:red;\">CONNECTION ERROR! </b>');
                getaccounts();
            }
        }
       );
    }
    else {

        getaccounts();
        msgbox('UPLOAD COMPLETED', '');
        setTimeout("showaccounts();", 1000);
    }
}

function b64toBlob(b64Data, contentType, sliceSize) {

    contentType = contentType || '';
    sliceSize = sliceSize || 512;

    var byteCharacters = atob(b64Data);
    var byteArrays = [];

    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);

        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        var byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
    }

    var blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

function logerrortoserver(errormsg) {
    $.ajax({
        type: "POST",
        url: localStorage.getItem('defaultserverpath') + "/uploaderror.aspx",
        data: {
            errormsg: errormsg
        },
        timeout: 20000,//six seconds
        success: function (result) {

        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    }
      );
}

function resizeBase64Img(base64) {
    var canvas = document.createElement("canvas");
    canvas.width = 500;
    canvas.height = 500;
    var context = canvas.getContext("2d");
    var deferred = $.Deferred();
    $("<img />").attr("src", base64).load(function () {
        context.scale(500 / this.width, 500 / this.height);
        context.drawImage(this, 0, 0);
        deferred.resolve(canvas.toDataURL());
    });
    return deferred.promise();
}