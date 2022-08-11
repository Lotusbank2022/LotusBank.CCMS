<%@ Page Title="User Group" Language="C#" MasterPageFile="~/application/MasterPage.master" AutoEventWireup="true" CodeFile="UserGroupList.aspx.cs" Inherits="UserGroupList" %>


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
            <asp:Button ID="exportbtn" Style="display: none" runat="server" OnClick="exportbtn_Click" />

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
                                                    <i class="fa fa-users fa-3x"></i>
                                                </td>

                                                <td>
                                                    <h2 id="h1" runat="server" class="modal-title">User Group</h2>
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
                                            </ul>
                                        </div>



                                        <div class="btn-group" style="margin-bottom: 5px;">
                                            <table>
                                                <tr>
                                                    <td style="width: 300px;">

                                                        <asp:TextBox ID="searchtxt" runat="server" placeholder="search information " class="form-control skipvalidation"></asp:TextBox>
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



                                        <div class="btn-group  pull-right" style="margin-bottom: 5px;">
                                            <button onclick="newrecord();" style="margin-right: 10px;" data-toggle="dropdown" class="btn btn-primary dropdown-toggle">
                                                New Record&nbsp;
                                                  <il class="fa fa-plus"></il>
                                            </button>

                                        </div>

                                        <asp:GridView ID="GridView1" class="table table-bordered" runat="server" AutoGenerateColumns="False" DataSourceID="EntityDataSource1" AllowPaging="True" HeaderStyle-BackColor="#F5F5F6" PageSize="5" EmptyDataText="No Record to display">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="40">
                                                    <HeaderTemplate>
                                                        <div id="mainchecker" onclick="checkall()" class="icheckbox_square-green gridcheckbox" style="position: relative;">
                                                        </div>

                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div class="icheckbox_square-green gridcheckbox childchecker" value="<%#Eval("Id") %>" style="position: relative;">
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="Id" HeaderText="#" ReadOnly="True" SortExpression="Id"></asp:BoundField>
                                                <asp:BoundField DataField="GroupName" HeaderText="Group Name" ReadOnly="True" SortExpression="GroupName"></asp:BoundField>
                                                <asp:BoundField DataField="Status" HeaderText="Status" ReadOnly="True" SortExpression="Status"></asp:BoundField>


                                                <asp:TemplateField ItemStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="btn-group">
                                                            <button data-toggle="dropdown" class="btn btn-default dropdown-toggle">
                                                                Action
                                                    <il class="fa fa-caret-down"></il>
                                                            </button>
                                                            <ul class="dropdown-menu">
                                                                <li><a onclick="view('<%#Eval("Id") %>')" class="dropdown-item" href="#">View</a></li>
                                                                <li><a onclick="edit('<%#Eval("Id") %>')" class="dropdown-item" href="#">Edit</a></li>
                                                            </ul>
                                                        </div>

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>

                                            <HeaderStyle BackColor="#F5F5F6" />
                                            <PagerSettings FirstPageText="&lt;&lt;" Mode="NumericFirstLast" />
                                            <PagerStyle CssClass="GridPager" HorizontalAlign="Center" />


                                        </asp:GridView>
                                        <asp:EntityDataSource runat="server" OnSelected="EntityDataSource1_Selected" OrderBy="it.Id" ID="EntityDataSource1" DefaultContainerName="CCMSEntities" ConnectionString="name=CCMSEntities" EnableFlattening="False" EntitySetName="UserGroup" Select="it.[Id], it.[GroupName],it.[Status]"></asp:EntityDataSource>

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
                                                    <i class="fa fa-users fa-3x"></i>
                                                </td>
                                                <td>
                                                    <h2 class="modal-title">UserGroup Details</h2>
                                                </td>
                                                <td style="text-align: right;"><i style="cursor: pointer" onclick="cancel();" class="fa fa-close fa-2x"></i></td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div class="ibox-content" style="padding-right: 10px; padding-left: 10px;">
                                        <div id="detailsregion" runat="server" style="padding: 10px;" class="row">

                                            <asp:HiddenField ID="recordid" runat="server" />

                                            <label>Group Name</label>
                                            <div class="form -group">
                                                <asp:TextBox ID="GroupName_db" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                            <br />


                                            <label>Default Page</label>
                                            <div class="form-group">
                                                <asp:DropDownList ID="defaultpage_db" runat="server" class="form-control">
                                                    <asp:ListItem Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <br />

                                            <label>Status</label>
                                            <div class="form-group">
                                                <asp:DropDownList ID="Status_db" runat="server" class="form-control">
                                                    <asp:ListItem Value=""></asp:ListItem>
                                                    <asp:ListItem Value="ACTIVE"> ACTIVE</asp:ListItem>
                                                    <asp:ListItem Value="DISABLED"> DISABLED</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <br />
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



    <script type="text/javascript" language="javascript">

        var currentscreen = 'browselink';

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
            document.getElementById('<%=GroupName_db.ClientID %>').value = '';
            document.getElementById('<%=defaultpage_db.ClientID %>').value = '';
            document.getElementById('<%=Status_db.ClientID %>').value = '';

            document.getElementById('<%=recordid.ClientID %>').value = '';

            document.getElementById('<%=savebtn.ClientID %>').style.display = 'inline';
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

        function exportrecord(format) {
            if (format == 'excel checked items') {
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




