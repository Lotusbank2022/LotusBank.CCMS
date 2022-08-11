<%@ Page Title="Customer Complaints" Language="C#" MasterPageFile="~/application/MasterPage.master" AutoEventWireup="true" CodeFile="customercomplaint.aspx.cs" Inherits="customercomplaint" %>
<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        body {
            font-family: Arial;
            font-size: 10pt;
        }

        .GridPager a, .GridPager span {
            display: block;
            padding-right: 10px;
            padding-left: 10px;
            padding-top: 5px;
            padding-bottom: 5px;
            margin: 2px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            background-color: #1ab394;
            color: white;
        }

        .GridPager a {
            background-color: white;
            color: black;
            border: 1px solid #969696;
        }

        .GridPager span {
            color: white;
            border: 1px solid #3AC0F2;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="updatepanel1" runat="server" UpdateMode="Conditional" OnLoad="OnLoad">

        <Triggers>
            <asp:PostBackTrigger ControlID="exportbtn" />
            
        </Triggers>

        <ContentTemplate>

            <asp:HiddenField ID="servermethod" runat="server" />
            <asp:HiddenField ID="serverparameter" runat="server" />
            <asp:Button ID="exportbtn" style="display:none" runat="server" OnClick="exportbtn_Click" />
            
            <div id="page-wrapper" class="gray-bg dashboard-1">

                <br />

                <div class="ibox-content product-box active" runat="server" id="slide1">

                    <div class="ibox-content" style="padding-top: 0px; padding-bottom: 0px;">


                        <div id="accordion">
                            <h4 id="browselink" class="accordion-toggle" style="display: none;">..</h4>
                            <div class="accordion-content default" style="display: none;">
                                <div class="ibox">
                                    <div class="ibox-title" style="border-style: none;">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td style="width: 40px;">
                                                   <i class="fa fa-file-code-o fa-3x"></i>
                                                </td>

                                                <td>
                                                    <h2 id="h1" runat="server" class="modal-title">Customer Complaints</h2>
                                                </td>
                                                <td style="text-align: right;"></td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div class="ibox-content" style="padding-right: 10px; padding-left: 10px;">

                                        <div class="btn-group" style="margin-bottom: 5px;">
                                            <button style="margin-right: 10px;" data-toggle="dropdown" class="btn btn-default dropdown-toggle">
                                                5
                                    <il class="fa fa-chevron-down"></il>
                                            </button>
                                            <ul class="dropdown-menu">
                                                <li><a onclick="pagesize(5);" class="dropdown-item" href="#">5</a></li>
                                                <li><a onclick="pagesize(10);" class="dropdown-item" href="#">10</a></li>
                                                <li><a onclick="pagesize(20);" class="dropdown-item" href="#">20</a></li>
                                                <li><a onclick="pagesize(30);" class="dropdown-item" href="#">30</a></li>
                                                <li><a onclick="pagesize(40);" class="dropdown-item" href="#">40</a></li>
                                                <li><a onclick="pagesize(50);" class="dropdown-item" href="#">50</a></li>
                                            </ul>

                                        </div>

                                        <div class="btn-group" style="margin-bottom: 5px;">
                                            <button style="margin-right: 10px;" data-toggle="dropdown" class="btn btn-default dropdown-toggle">
                                                Export&nbsp;
                                    <il class="fa fa-download"></il>
                                            </button>
                                            <ul class="dropdown-menu">
                                                <li><a onclick="exportrecord('excel')" class="dropdown-item" href="#">Excel</a></li>
                                                <li><a onclick="exportrecord('excel checked items')" class="dropdown-item" href="#">Excel (Checked Items)</a></li>

                                                <li><hr style="margin-top:1px; margin-bottom:1px;"></li>

                                                <li><a onclick="exportrecord('csv')" class="dropdown-item" href="#">CSV</a></li>
                                                <li><a onclick="exportrecord('csv checked items')" class="dropdown-item" href="#">CSV (Checked Items)</a></li>
                                            </ul>
                                        </div>
                                       
                                        <div class="btn-group" style="margin-bottom: 5px;">
                                            <table>
                                                <tr>
                                                    <td style="width: 300px;">

                                                        <asp:TextBox ID="searchtxt" runat="server" placeholder="search information " class="form-control skipvalidation" AutoComplete="off"></asp:TextBox>
                                                    </td>
                                                    <td>&nbsp;</td>

                                                    <td>
                                                        <button onclick="search();" style="margin-right: 10px;" data-toggle="dropdown" class="btn btn-danger dropdown-toggle">
                                                            Search &nbsp;
                                                             <il class="fa fa-search"></il>
                                                        </button>
                                                        <asp:Label ID="countlbl" Font-Bold="true" Text="0" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                       
                                        <div id="divNR" runat="server" class="btn-group  pull-right" style="margin-bottom: 5px;">
                                            <button onclick="newrecord();" style="margin-right: 10px;" data-toggle="dropdown" class="btn btn-primary dropdown-toggle">
                                                New Record&nbsp;
                                                  <il class="fa fa-plus"></il>
                                            </button>
                                        </div>

                                        <asp:GridView ID="GridView1" class="table table-bordered" runat="server" AutoGenerateColumns="False" DataSourceID="EntityDataSource1" AllowPaging="True" HeaderStyle-BackColor="#F5F5F6" PageSize="5"  EmptyDataText="No Record to display" OnPageIndexChanging="GridView1_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="40">
                                                    <HeaderTemplate>
                                                       <div id="mainchecker" onclick="checkall()" class="icheckbox_square-green gridcheckbox" style="position: relative;">
                                                        </div>

                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div class="icheckbox_square-green gridcheckbox childchecker"  value="<%#Eval("ID") %>"  style="position: relative;">
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                  
                                                 <asp:BoundField DataField="ID" HeaderText="#" ReadOnly="True" SortExpression="ID"></asp:BoundField>
                                                 <asp:BoundField DataField="Account_Number" HeaderText="Account Number" ReadOnly="True" SortExpression="Account_Number"></asp:BoundField>
                                                 <asp:BoundField DataField="Tracking_Reference_Number" HeaderText="Ref Number" ReadOnly="True" SortExpression="Tracking_Reference_Number"></asp:BoundField>
                                                 <asp:BoundField DataField="Last_Name" HeaderText="Last Name" ReadOnly="True" SortExpression="Last_Name"></asp:BoundField>
                                                 <asp:BoundField DataField="First_Name" HeaderText="First Name" ReadOnly="True" SortExpression="First_Name"></asp:BoundField>
                                                 <asp:BoundField DataField="Middle_Name" HeaderText="Middle Name" ReadOnly="True" SortExpression="Middle_Name"></asp:BoundField>
                                                 <asp:BoundField DataField="Status_Of_Complaint" HeaderText="Complaint Status" ReadOnly="True" SortExpression="Status_Of_Complaint"></asp:BoundField>

                                               
                                                  <asp:TemplateField ItemStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="btn-group">
                                                            <button data-toggle="dropdown" class="btn btn-default dropdown-toggle">
                                                                Action
                                                    <il class="fa fa-caret-down"></il>
                                                            </button>
                                                            <ul class="dropdown-menu">
                                                               <li><a onclick = "view('<%#Eval("ID") %>')" class="dropdown-item" href="#">View</a></li>
                                                               <li><a onclick = "edit('<%#Eval("ID") %>')" class="dropdown-item" href="#">Edit</a></li>
                                                                                      
                                                             </ul>
                                                        </div>

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>

                                            <HeaderStyle BackColor="#F5F5F6" />
                                            <PagerSettings FirstPageText="&lt;&lt;" Mode="NumericFirstLast" />
                                            <PagerStyle CssClass="GridPager" HorizontalAlign="Center" />


                                        </asp:GridView>
                                        <asp:EntityDataSource runat="server" OnSelected="EntityDataSource1_Selected" OrderBy="it.ID" ID="EntityDataSource1" DefaultContainerName="CCMSEntities" ConnectionString="name=CCMSEntities" EnableFlattening="False" EntitySetName="CustomerComplaints" Select="it.[ID], it.[Account_Number],it.[Tracking_Reference_Number],it.[Last_Name],it.[First_Name],it.[Middle_Name],it.[Status_Of_Complaint]"></asp:EntityDataSource>

                                    </div>

                                </div>
                            </div>

                            <h4 id="formlink" class="accordion-toggle" style="display: none;">..</h4>
                            <div class="accordion-content" style="display: none;">
                                <div class="ibox">
                                    <div class="ibox-title" style="border-style: none;">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td style="width: 40px;">
                                                    <i class="fa fa-file-code-o fa-3x"></i>
                                                </td>
                                                <td>
                                                    <h2 class="modal-title">Form Details</h2>
                                                </td>
                                                   <td style="text-align: right;"><i style="cursor:pointer" onclick="cancel();" class="fa fa-close fa-2x"></i></td>
                                              </tr>
                                        </table>
                                    </div>

                                    <div class="ibox-content" style="padding-right: 10px; padding-left: 10px;">
                                             <div id="detailsregion" runat="server" style="padding: 10px; " class="row">

                                                <asp:HiddenField ID="recordid" runat="server" />

                                                 <div class="row">
                                                     <div class="col-md-3">
                                                         <div class="form-group">
                                                             <label>Account Number</label>
                                                             <asp:TextBox ID="Account_Number_db" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                             <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass ="btn btn-primary" />
                                                         </div>
                                                     </div>
                                                  <%--   <div class="col-md-2">   
                                                     <div class="form-group">                                                         
                                                         <asp:Button ID="Button1" runat="server" Text="Search" />                                                             
                                                     </div>   
                                                         </div> --%>  
                                                  </div>
                                                 <hr />

                                                 <div class="row">
                                                     <div class="col-md-4">

                                                         <label>Tracking Reference Number</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Tracking_Reference" ReadOnly ="true"  runat="server" class="form-control"></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                         <label>Account Number</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"  ID = "Account_Number"  runat="server" class="form-control"></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Account Name</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"  ID = "Name_Of_Complainant_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Account Branch Name</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"   ID = "Account_Branch_Name_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>First Name</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "First_Name_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Middle Name</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"    ID = "Middle_Name_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Last Name</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"   ID = "Last_Name_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Account Type</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "Account_Type_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Account Currency</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "Account_Currency_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                         <label>Identification (BVN/TIN)</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"  ID = "Unique_Identification_No_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Email Address</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="false"   ID = "Email_Address_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>                                                                                                                                                                          
                                                         </div>

                                                     <div class="col-md-4"> 

                                                         <label>Address1</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"    ID = "Address1_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>
                                                         
                                                         <label>Address2</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ReadOnly ="true"  ID = "Address2_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>                                                

                                                 <label>City</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "City_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>State</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "State_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Country</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "Country_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Phone Number</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="false"   ID = "Cell_Phone_Number_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>


                                                         <label>Office Number</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="false"   ID = "Office_Number_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Postal Code</label>
                                                           <div class="form-group">
                                                               <asp:TextBox ReadOnly ="true"   ID = "Postal_Code_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                         <label>Type of Client</label>
                                                             <div class="form-group">          
                                                                 <asp:DropDownList ID = "Type_of_client_db"  runat = "server" class="form-control ">             
                                                                     <asp:ListItem Value = "--Select--" ></asp:ListItem>              
                                                                     <asp:ListItem Value = "INDV">INDIVIDUAL</asp:ListItem>              
                                                                     <asp:ListItem Value = "CORP">CORPORATE</asp:ListItem>                                                                                        
                                                                 </asp:DropDownList>    
                                                             </div>
                                                            <br/>
                                                         
                                                         <label>Complaint Channel</label>
                                                             <div class="form-group">          
                                                                 <asp:DropDownList ID = "Complaint_Channel_db"  runat = "server" class="form-control ">             
                                                                     <asp:ListItem Value = "--Select--" ></asp:ListItem>             
                                                                     <asp:ListItem Value = "LETTER">LETTER</asp:ListItem>              
                                                                     <asp:ListItem Value = "EMAIL">EMAIL</asp:ListItem>              
                                                                     <asp:ListItem Value = "WALK_IN">WALK IN</asp:ListItem>              
                                                                     <asp:ListItem Value = "WEBSITE">WEBSITE</asp:ListItem>              
                                                                     <asp:ListItem Value = "MOBILE_APP">MOBILE APP</asp:ListItem>              
                                                                     <asp:ListItem Value = "SOCIAL_MEDIA">SOCIAL MEDIA</asp:ListItem>              
                                                                     <asp:ListItem Value = "PHONE">PHONE</asp:ListItem>              
                                                                     <asp:ListItem Value = "CHAT">CHAT</asp:ListItem>              
                                                                     <asp:ListItem Value = "OTHERS">OTHERS</asp:ListItem>          
                                                                 </asp:DropDownList>  
                                                                 
                                                             </div>
                                                            <br/>  
                                                          
                                                                                                         
                                                         <label>Description of Resolution (Outcome)</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Description_of_Resolution"  TextMode="MultiLine"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                         </div>
                                                     <br/> 
                                                     <div class="col-md-4">                                                                 
                                                         <label>Complaint Category</label>    
                                                         <div class="form-group">          
                                                             <asp:DropDownList ID = "Complaint_Category_db"  runat = "server" class="form-control"
                                                                 AutoPostBack="True" OnSelectedIndexChanged="Complaint_Category_db_SelectedIndexChanged">                                                                                           
                                                             </asp:DropDownList>                                                               
                                                         </div>
                                                        <br/>      

                                                         <label>Complaint Sub-Category</label>    
                                                         <div class="form-group">          
                                                             <asp:DropDownList ID = "Complaint_Category_Code_db"  runat = "server" class="form-control ">             
                                                                 <%--<asp:ListItem Value = "" ></asp:ListItem>              
                                                                 <asp:ListItem Value = "A001">A001</asp:ListItem> --%>         
                                                             </asp:DropDownList>  
                                                           
                                                         </div>
                                                        <br/>                      

                                                         <label>Date complaint was received</label>
                                                           <div class="form-group">
                                                               <asp:TextBox  ID = "dtReceivedPicker"  runat="server" class="form-control " AutoComplete="off"></asp:TextBox> 
                                                             </div>
                                                       
                                                             <br/>  
                                                                                                         
                                                         <label>Subject Of Complaint</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Subject_Of_Complaint_db"  TextMode="MultiLine"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                        
                                                             <br/>    
                                                         
                                                         <label>Complaint Description</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Complaint_Description_db"  TextMode="MultiLine"  runat="server" class="form-control "></asp:TextBox> 
                                                             
                                                             </div>
                                                             <br/>                                               

                                                 <label>Amount In Dispute</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Amount_In_Dispute_db"  TextMode="Number"  runat="server" class="form-control "></asp:TextBox> 
                                                              
                                                             </div>
                                                             <br/>

                                                 <label>Amount Refunded to Petitioner</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Amount_Refunded_Petitioner_db"  TextMode="Number"  runat="server" class="form-control "></asp:TextBox>                                                                
                                                             </div>
                                                             <br/>

                                                 <label>Amount Recovered by Bank</label>
                                                           <div class="form -group">
                                                               <asp:TextBox    ID = "Amount_Recovered_by_Bank_db"  TextMode="Number"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>

                                                 <label>Action Taken</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Action_Taken_db"  runat="server" class="form-control "></asp:TextBox> 
                                                               
                                                             </div>
                                                             <br/>

                                                         <label>Status of Complaint</label>
                                                             <div class="form-group">          
                                                                 <asp:DropDownList ID = "status_of_complaint"  runat = "server" class="form-control"
                                                                     AutoPostBack="True" OnSelectedIndexChanged="status_of_complaint_SelectedIndexChanged">            
                                                                     <asp:ListItem Value = "--Select--" ></asp:ListItem>
                                                                     <asp:ListItem Value = "NEW">NEW</asp:ListItem>
                                                                     <asp:ListItem Value = "ASSIGNED">ASSIGNED</asp:ListItem>
                                                                     <asp:ListItem Value = "PENDING_INPUT">PENDING_INPUT</asp:ListItem>
                                                                     <asp:ListItem Value = "RESOLVED">RESOLVED</asp:ListItem>              
                                                                     <asp:ListItem Value = "CLOSED">CLOSED</asp:ListItem>
                                                                     <asp:ListItem Value = "REJECTED">REJECTED</asp:ListItem>
                                                                 </asp:DropDownList> 
                                                                
                                                             </div>
                                                            <br/>   
                                                         
                                                         <div class="row" runat="server" visible="false" id="divDateClosed">
                                                            <label>Date complaint was closed/resolved</label>
                                                            <div class="form-group">
                                                               <asp:TextBox  ID= "dtClosedPicker"  runat="server" class="form-control" AutoComplete="off"></asp:TextBox>                                                                 
                                                             </div>
                                                             <br/>  
                                                        </div>   

                                                        <label>Remarks by Bank</label>
                                                           <div class="form-group">
                                                               <asp:TextBox    ID = "Remarks_by_Bank_db"  runat="server" class="form-control "></asp:TextBox> 
                                                             </div>
                                                             <br/>
                                                         </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" onclick="cancel();" style="padding-left: 50px; padding-right: 50px; padding-top: 10px; padding-bottom: 10px;" class="btn btn-white" data-dismiss="modal">Close</button>

                                            <button type="button" id="savebtn" runat="server" onclick="saverecord();" style="padding-left: 50px; padding-right: 50px; padding-top: 10px; padding-bottom: 10px;" class="btn btn-sm btn-primary pull-center m-t-n-xs">Save</button>

                                        </div>
                                    </div>
                                    </div>

                             
                            </div>

                        </div>
                    </div>
                </div>
                <br />
                <br />
                <br />

                <div class="footer">
                    <table style="width: 100%">
                        <tr>
                            <td style="text-align: left;"><strong>Copyright</strong> SunTrust Bank &copy; <span id="yearlbl" runat="server"></span></td>
                        </tr>
                    </table>
                    <div>
                    </div>

                    <div class="pull-center m-t-n-xs">
                    </div>
                </div>
            </div>
</ContentTemplate>

    </asp:UpdatePanel>

    <script src="js/jquery-1.12.4.js"></script>
    <script src="js/jquery-ui.js"></script> 

    <%--<link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/jquery-ui.min.js"></script>--%>
    <script>
            $(function () {                    
                $("input[id$='dtReceivedPicker']").datepicker({
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,  
                    changeYear: true,  
                    yearRange: '2018:2100'  
                });  
            })

            $(function () {
                $("input[id$='dtClosedPicker']").datepicker({  
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '2018:2100'
                });
            })
     

        var currentscreen = 'browselink';

        $(document).ready(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler)                       
        });

        function EndRequestHandler(sender, args) {
            $("input[id$='dtReceivedPicker']").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '2018:2100'
            });

            $("input[id$='dtClosedPicker']").datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '2018:2100'
            });
        }

        function pageLoad() {

              $('.childchecker').each(
                function () {
                    $(this).on('click', function () {
                        if ($(this).hasClass('checked')) {
                            $(this).removeClass('checked');
                        }
                        else
                            $(this).addClass('checked');
                         }
                     );
                   }
                );

            $('#accordion').find('.accordion-toggle').click(function () {
                $(this).next().slideToggle('fast');
                $(".accordion-content").not($(this).next()).slideUp('fast', function () { $(this).next().hide() });
            });

            hideloader();
            showscreen(currentscreen);
            $('.GridPager a').click(function () { showloader(); });           
        }
        

        function search() {
            callserver('search');
        }

               function newrecord() {
                   //clear form
                   
                   document.getElementById('<%=Account_Number.ClientID %>').value = '';
                   document.getElementById('<%=Complaint_Category_db.ClientID %>').value = '';
                   document.getElementById('<%=Tracking_Reference.ClientID %>').value = '';
                   document.getElementById('<%=dtReceivedPicker.ClientID %>').value = '';
                   <%--document.getElementById('<%=dtClosedPicker.ClientID %>').value = '';--%>
                  document.getElementById('<%=Account_Number_db.ClientID %>').value = '';
                  document.getElementById('<%=Type_of_client_db.ClientID %>').value = '';
                  document.getElementById('<%=Name_Of_Complainant_db.ClientID %>').value = '';
                  document.getElementById('<%=Account_Branch_Name_db.ClientID %>').value = '';
                  document.getElementById('<%=First_Name_db.ClientID %>').value = '';
                  document.getElementById('<%=Middle_Name_db.ClientID %>').value = '';
                  document.getElementById('<%=Last_Name_db.ClientID %>').value = '';
                  document.getElementById('<%=Account_Type_db.ClientID %>').value = '';
                  document.getElementById('<%=Account_Currency_db.ClientID %>').value = '';
                  document.getElementById('<%=Unique_Identification_No_db.ClientID %>').value = '';
                  document.getElementById('<%=Email_Address_db.ClientID %>').value = '';
                  document.getElementById('<%=Address1_db.ClientID %>').value = '';
                  document.getElementById('<%=Address2_db.ClientID %>').value = '';
                  document.getElementById('<%=City_db.ClientID %>').value = '';
                  document.getElementById('<%=State_db.ClientID %>').value = '';
                  document.getElementById('<%=Country_db.ClientID %>').value = '';
                  document.getElementById('<%=Cell_Phone_Number_db.ClientID %>').value = '';
                  document.getElementById('<%=Office_Number_db.ClientID %>').value = '';
                  document.getElementById('<%=Postal_Code_db.ClientID %>').value = '';
                  document.getElementById('<%=Complaint_Channel_db.ClientID %>').value = '';
                  document.getElementById('<%=Complaint_Category_Code_db.ClientID %>').value = '';
                  document.getElementById('<%=Subject_Of_Complaint_db.ClientID %>').value = '';
                  document.getElementById('<%=Complaint_Description_db.ClientID %>').value = '';
                  document.getElementById('<%=Amount_In_Dispute_db.ClientID %>').value = '';
                  document.getElementById('<%=Amount_Refunded_Petitioner_db.ClientID %>').value = '';
                  document.getElementById('<%=Amount_Recovered_by_Bank_db.ClientID %>').value = '';
                  document.getElementById('<%=Action_Taken_db.ClientID %>').value = '';
                  document.getElementById('<%=Remarks_by_Bank_db.ClientID %>').value = '';

            document.getElementById('<%=recordid.ClientID %>').value = '';

            document.getElementById('<%=savebtn.ClientID %>').style.display='inline';
            showscreen('formlink');
        }

                function edit(id) {
            callserver('edit', id);
        }

        

                function view(id) {
            callserver('view', id);
        }
      
        

         function saverecord() {
            var res = validateallinputs();

            if (res == false) {
                showmsg('Missing Information', 'Please enter information in all fields highlighted in red', 'warning');
                return false;
            }
            callserver('submitrecord', '');
            return true;
        }

                function cancel() {
            showscreen('browselink');
        }

        function pagesize(size) {
            callserver('pagesize', size);
        }

        function exportrecord(format) 
        {
            if (format == 'excel checked items' || format == 'csv checked items') {
                var ids = '';
                $('.childchecker').each(function () {
                    if ($(this).hasClass('checked')) { ids += $(this).attr('value') + ','; }
                });

                if (ids == '') {
                    showmsg('Error', 'You must check one or more records', 'warning');
                    return false;
                }
                
		document.getElementById('<%= serverparameter.ClientID %>').value = format + '|' + ids;
		document.getElementById('<%= exportbtn.ClientID %>').click();                     
            }
            else {
                document.getElementById('<%= serverparameter.ClientID %>').value = format;
                document.getElementById('<%= exportbtn.ClientID %>').click();
            }
        }

        

        function checkall() {            

           if ($('#mainchecker').hasClass('checked') == false) {
                $('#mainchecker').addClass('checked');
                $('.childchecker').addClass('checked');
            }
            else {
                $('#mainchecker').removeClass('checked');
                $('.childchecker').removeClass('checked');
            }      
        }

        function callserver(method, params) {
            showloader();
            document.getElementById('<%= servermethod.ClientID %>').value = method;
            document.getElementById('<%= serverparameter.ClientID %>').value = params;
            __doPostBack("<%= updatepanel1.UniqueID %>", '');
        }

        function showscreen(screen) {
            currentscreen = screen;
            $('#' + currentscreen).click();
        }


    </script>


</asp:Content>




