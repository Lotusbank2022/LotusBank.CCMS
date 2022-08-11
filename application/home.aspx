<%@ Page Title="Search Customer" Language="C#" MasterPageFile="~/application/MasterPage.master" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    

    <asp:UpdatePanel ID="updatepanel1" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="progressstatus" runat="server" />

            <div id="page-wrapper" style=" padding-top:10px;" class="gray-bg dashboard-1">

                 
                
                <div   class="ibox-content product-box active" runat="server" id="slide1">
                    <div  class="ibox-title">
                      <table style="width: 100%; margin-top:50px; margin-bottom:150px;">
                            <tr>
                                 
                                <td>
                                  <div style="text-align:center; color:red;">
                                      <img src="img/landing/word_map.png" />
                                      <label style="position:absolute; left:40%; top:30%; font-size:30px;">Customer Complaints Management System</label>
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

