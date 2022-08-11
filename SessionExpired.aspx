<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SessionExpired.aspx.cs" Inherits="SessionExpired" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link rel="icon" href="framework7/img/suntrusticon.png">
    <link href="custom/css/main.css" rel="stylesheet" />
    <script src="custom/js/script.js"></script>

    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <title>eBills Pay | LOGIN </title>

    <link href="framework7/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="framework7/bootstrap/css/style.css" rel="stylesheet" />
    <link href="framework7/bootstrap/css/plugins/sweetalert/sweetalert.css" rel="stylesheet" />

    <script src="framework7/bootstrap/js/jquery-2.1.1.js"></script>
    <script src="framework7/bootstrap/js/bootstrap.min.js"></script>
    <script src="framework7/bootstrap/js/plugins/sweetalert/sweetalert.min.js"></script>


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
                                <img style="width: 40px;" class="img-circle" src="framework7/img/suntrusticon.png" />
                                <b style="color: green;">SunTrust Bank</b>
                            </h2>
                            <br />
                            <h2 style="width: 100%; text-align: center;">
                                <b>Customer Complaints Management System</b>
                            </h2>
                            <br />

                            <div id="signin_div" runat="server">

                                <div class="row">

                                    <div class="col-lg-12">

                                        <asp:Label ID="lblError" runat="server" ForeColor="Red" style="font-weight: 700; font-size: large"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:LinkButton ID="lnkLogin" runat="server" PostBackUrl="~/default.aspx">Go to Login Page</asp:LinkButton>

                                        <br />

                                        <div id="signinloaderdiv" style="display: none">
                                            <div class="sk-spinner sk-spinner-wave">
                                                <div class="sk-rect1"></div>
                                                <div class="sk-rect2"></div>
                                                <div class="sk-rect3"></div>
                                                <div class="sk-rect4"></div>
                                                <div class="sk-rect5"></div>
                                            </div>
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
                        <small>© 2018</small>  SunTrust Bank Nigeria
                    </div>
                    <div class="col-md-6 text-right">
                    </div>
                </div>
            </div>

        </div>
    
          
          
       
    </form>
</body>




</html>
