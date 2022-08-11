<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link rel="icon" href="framework7/img/lotus_logo.png" />
    <link href="custom/css/main.css" rel="stylesheet" />
    <script src="custom/js/script.js"></script>

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Customer Complaints Management System | LOGIN </title>

    <link href="framework7/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="framework7/bootstrap/css/style.css" rel="stylesheet" />
    <link href="framework7/bootstrap/css/plugins/sweetalert/sweetalert.css" rel="stylesheet" />

    <script src="framework7/bootstrap/js/jquery-2.1.1.js"></script>
    <script src="framework7/bootstrap/js/bootstrap.min.js"></script>
    <script src="framework7/bootstrap/js/plugins/sweetalert/sweetalert.min.js"></script>

    <script type="text/javascript">

        function signinbtn_click() {

            var res = validateallinputs();
            if (res == true) {
                showloader();
                return true;
            }
            showmsg('Missing Information', 'Please enter mobile number and password to sign In', 'warning');
            return false;
        }

        function signinprofile(id)
        {
            showloader();
            document.getElementById('selectedprofile').value = id;
            document.getElementById('signinwithprofilebtn').click();
        }
        
        


    </script>

</head>
<body style="background-color: #f3f3f4;">
    <form id="form1" runat="server">

        <asp:HiddenField ID="selectedprofile" runat="server" />
       <button id="loaderbtn" style="display: none;" type="button" class="btn btn-primary" data-toggle="modal" data-target="#loaderpopup"></button>
        <div class="modal inmodal fade" id="loaderpopup" data-backdrop="static" tabindex="-1" role="dialog" aria-hidden="true" style="display: none;">
            <div class="modal-dialog modal-sm">
                <div class="modal-content">
                    <div class="modal-header">
                        Please wait..
                    </div>
                </div>
            </div>
        </div>

        <div class="passwordBox animated fadeInDown" style="padding-top: 100px;">

            <div id="signinpage">
                <div class="row">
                    <div class="col-md-12">
                        <div class="ibox-content">

                            <h2 class="font-bold">
                                <img style="width: 100px; height: 100px" class="img-circle" src="framework7/img/lotus_logo.png" />
                                <b style="color: darkblue; text-align: center;">Lotus Bank</b>
                            </h2>
                            <br />
                            <h2 style="width: 100%; text-align: center; font-size: 14px">
                                <b>Customer Complaints Management System</b>
                            </h2>
                            <br />

                            <div id="signin_div" runat="server">
                                <p>
                                    Enter your login credential to sign in.
                                </p>

                                <div class="row">

                                    <div class="col-lg-12">

                                        <div class="form-group">
                                            <asp:TextBox runat="server" ID="useridtxt" autocomplete="off" class="form-control" placeholder="User ID"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <asp:TextBox runat="server" ID="passwordtxt" TextMode="Password" autocomplete="off" class="form-control" placeholder="Password"></asp:TextBox>
                                        </div>

                                        <br />
                                        <div id="signinbtndiv">
                                            <asp:Button ID="signinbtn" Style="float: none; background-color: #20d5d5; color: white;" class="signoutbtn" Width="100%" runat="server" Text=" Sign In " OnClientClick="return signinbtn_click();" OnClick="signinbtn_Click" />
                                        </div>

                                        <div id="signinloaderdiv" style="display: none">
                                            <div class="sk-spinner sk-spinner-wave">
                                                <div class="sk-rect1"></div>
                                                <div class="sk-rect2"></div>
                                                <div class="sk-rect3"></div>
                                                <div class="sk-rect4"></div>
                                                <div class="sk-rect5"></div>
                                            </div>
                                        </div>

                                        <hr />
                                        <div style="text-align: center; width: 100%; padding-bottom: 20px;"><a href="Customer Complaints Management System.pdf" target="_blank">User Manual</a></div>
                                    </div>
                                </div>
                            </div>

                            <div id="select_profile" runat="server" visible="false" style="padding: 10px;">
                                <p style="text-align:center">
                                   <b> Select profile to sign in with.</b>
                                </p>

                                <div class="row">

                                    <div class="col-lg-12">
                                        <div id="profilelist" runat="server">

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-6">
                        <small>© <asp:Label ID="thisyear" Font-Bold="false" Text="0" runat="server"></asp:Label></small>  Lotus Bank
                    </div>
                    <div class="col-md-6 text-right">
                    </div>
                </div>
            </div>

        </div>
    
          
          <asp:Button ID="signinwithprofilebtn"  runat="server" OnClick="signinwithprofilebtn_Click" style="display:none;" />
       
    </form>
</body>




</html>
