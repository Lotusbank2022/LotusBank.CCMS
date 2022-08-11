<%@ Page Title="Search Customer" Language="C#" MasterPageFile="~/application/MasterPage.master" AutoEventWireup="true" CodeFile="accessdenied.aspx.cs" Inherits="accessdenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    

    <asp:UpdatePanel ID="updatepanel1" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="progressstatus" runat="server" />

            <div id="page-wrapper" class="gray-bg dashboard-1">

                <div id="pagebanner" class="row wrapper border-bottom white-bg page-heading">
                    
                </div>
                <div class="spacer"></div>

                <div class="ibox-content product-box active" runat="server" id="slide1">
                    <div class="ibox-title">
                      <table style="width: 100%; margin-top:200px; margin-bottom:230px;">
                            <tr>
                                 
                                <td>
                                  <div style="text-align:center; color:red;">
                                     <i class="fa fa-lock fa-5x"></i>   <h1 id="headerbanner3" runat="server" class="modal-title">&nbsp;&nbsp;ACCESS DENIED</h1>                                
                                  </div>  

                                </td>
                                 
                            </tr>
                        </table>  

                    </div>
                    <div class="ibox-content">

                         
                    </div>
                </div>

 

                
            </div>

        </ContentTemplate>

    </asp:UpdatePanel>
    
</asp:Content>

