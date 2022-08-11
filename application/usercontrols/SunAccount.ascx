<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SunAccount.ascx.cs" Inherits="application_SunAccount" %>

<script type="text/javascript" language="javascript">

    function sunaccount_getaccountname() {

        if (document.getElementById('<%=acctno.ClientID %>').value.length == 10) {

            sunaccount_show_ajaxautoloader();

            var data = { "accountnumber": document.getElementById('<%=acctno.ClientID %>').value };
            callwebmethod('utility.aspx/getaccountname',
                JSON.stringify(data),
                function (data) {
                    sunaccount_hide_ajaxautoloader();
                    if (data.d.responsecode == '00') {
                        document.getElementById('<%=acctname.ClientID %>').innerText = data.d.responsemessage;
                          document.getElementById('<%=acctname.ClientID %>').style.color = 'black';
                    }
                    else {
                        document.getElementById('<%=acctname.ClientID %>').innerText = data.d.responsemessage
                        document.getElementById('<%=acctname.ClientID %>').style.color = 'red';
                    }
                },
                    function (xhr, error) { sunaccount_hide_ajaxautoloader(); });
            }
            else {
                document.getElementById('<%=acctname.ClientID %>').value = '';
        }

    }

    function sunaccount_show_ajaxautoloader() {
        document.getElementById('<%=acctno.ClientID %>').style.backgroundImage = 'url(img/loader.gif)';
    }

    function sunaccount_hide_ajaxautoloader() {
        document.getElementById('<%=acctno.ClientID %>').style.backgroundImage = 'none';
    }

</script>


<label>Account Number</label>
<div class="form -group">
    <asp:TextBox ID="acctno" onkeyup="sunaccount_getaccountname();" runat="server" class="form-control compulsory" Style="background-repeat: no-repeat; background-position: right;"></asp:TextBox>
</div>
<div style="width: 100%; text-align: center;">
    <asp:Label runat="server" ID="acctname">...</asp:Label></div>



