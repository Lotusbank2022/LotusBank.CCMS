using EF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

public partial class sla_status : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Thread.Sleep(1000);
            clsUser oUser = new clsUser();
            if (Session["TellerID"] == null)
            {
                Response.Redirect("../SessionExpired.aspx?err=timedout", false);
            }

            if (!oUser.FetchUser(Session["TellerID"].ToString(), Session["SessionID"].ToString()))
            {
                if (!oUser.hasException)
                {
                    Response.Redirect("../SessionExpired.aspx?err=concurrentlogin", false);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error', 'Error Occurred. Please contact administrator', 'error'); ", true);
                    return;
                }
            }
            else
            {
                if (!IsPostBack)
                {
                    search();
                    loadComplaintCategory();
                }
            }
        }
        catch(Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }
    
    protected void OnLoad(object sender, EventArgs e)
    {
        switch (servermethod.Value.ToLower())
        {
            case "pagesize": pagesize(); break;
           
            case "search": search(); break;

             case "view": view(); break;

             case "edit": edit(); break;
        }
        servermethod.Value = "";
    }

    protected void pagesize()
    {
       try
       {
          GridView1.PageSize = int.Parse(serverparameter.Value);
          search();
       }catch{}
    }

     
    protected void search()
    {
       try
       {
            if (searchtxt.SelectedIndex != 0)
            {
                EntityDataSource1.Where = " (it.[SLA_STATUS] like '%" + searchtxt.SelectedValue + "%')";
            }
            //else
            //    EntityDataSource1.Where = " (it.[SLA_STATUS] like 'SLA VIOLATED' || it.[SLA_STATUS] like 'WITHIN SLA' || it.[SLA_STATUS] like 'ABOUT TO BREACH SLA' || it.[SLA_STATUS] like 'RESOLVED')";

            GridView1.DataBind();
       }catch(Exception ex){}
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        search();
        GridView1.PageIndex = e.NewPageIndex;
        GridView1.DataBind();
    }

        protected void view()
    {
        getdetails();
        savebtn.Style.Add("display","none");
    }

   
        protected void edit()
    {
        getdetails();
        savebtn.Style.Add("display","inline");
    }



    protected void status_of_complaint_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (status_of_complaint.SelectedValue == "CLOSED" || status_of_complaint.SelectedValue == "RESOLVED")
        {
            divDateClosed.Visible = true;
        }
        else divDateClosed.Visible = false;
    }

    private string matchPhoneNumber(string phoneNumber)
    {
        //<CELL_PHONE_NUMBER>(234) 706-2448095</CELL_PHONE_NUMBER>
        if (phoneNumber.Length == 11)
        {            
            phoneNumber = string.Format("({0}) {1}-{2}", "234", phoneNumber.Substring(1, 3), phoneNumber.Substring(4));
        }
        else if (phoneNumber.Length == 13)
        {
            phoneNumber = string.Format("({0}) {1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6));
        }
        else
            phoneNumber = "";

        return phoneNumber;
    }

    private bool validateEmail(string Email_Address_db)
    {
        var pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        bool isValid = Regex.IsMatch(Email_Address_db, pattern);

        return isValid;
    }

    private void loadComplaintCategory()
    {
        Complaint_Category_db.Items.Add("--Select--");
        try
        {
            using (var dbContext = new EF.CCMSEntities())
            {

                var query = (from d in dbContext.ComplaintsCategory
                             select new
                             {
                                 d.CategoryName,
                                 d.CategoryId
                             }).ToList();

                foreach (var item in query)
                {
                    Complaint_Category_db.Items.Add(new ListItem(item.CategoryName, item.CategoryId.ToString()));
                }

            }
        }
        catch (Exception ex) { }
    }

    private void loadComplaintSubCategory(string category)
    {
        Complaint_Category_Code_db.Items.Add("--Select--");
        try
        {
            using (var dbContext = new EF.CCMSEntities())
            {
                var query = dbContext.ComplaintsCategoryMatrix.Where(d => d.CategoryId.ToString() == category).ToList();

                foreach (var item in query)
                {
                    Complaint_Category_Code_db.Items.Add(new ListItem(item.SubCategory, item.CBNCode));
                }

            }
        }
        catch (Exception ex) { }
    }
    protected void Complaint_Category_db_SelectedIndexChanged(object sender, EventArgs e)
    {
        Complaint_Category_Code_db.Items.Clear();
        loadComplaintSubCategory(Complaint_Category_db.SelectedItem.Value);
    }

    public void getdetails()
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                var trackRef = serverparameter.Value.ToString();
                List<CustomerComplaints> Record = (from d in context.CustomerComplaints where d.Tracking_Reference_Number == trackRef select d).ToList();

                if (Record.Count > 0)
                {
                    recordid.Value= Record[0].ID.ToString();
                    Tracking_Reference.Text= (Record[0].Tracking_Reference_Number == null) ? "" : Record[0].Tracking_Reference_Number.ToString();
                    Account_Number.Text = (Record[0].Account_Number == null) ? "" : Record[0].Account_Number.ToString();
                    Name_Of_Complainant_db.Text = (Record[0].Name_Of_Complainant==null) ? "":Record[0].Name_Of_Complainant.ToString();
                    Account_Branch_Name_db.Text = (Record[0].Account_Branch_Name==null) ? "":Record[0].Account_Branch_Name.ToString();
                    First_Name_db.Text = (Record[0].First_Name==null) ? "":Record[0].First_Name.ToString();
                    Middle_Name_db.Text = (Record[0].Middle_Name==null) ? "":Record[0].Middle_Name.ToString();
                    Last_Name_db.Text = (Record[0].Last_Name==null) ? "":Record[0].Last_Name.ToString();
                    Account_Type_db.Text = (Record[0].Account_Type==null) ? "":Record[0].Account_Type.ToString();
                    Account_Currency_db.Text = (Record[0].Account_Currency==null) ? "":Record[0].Account_Currency.ToString();
                    Unique_Identification_No_db.Text = (Record[0].Unique_Identification_No==null) ? "":Record[0].Unique_Identification_No.ToString();
                    Email_Address_db.Text = (Record[0].Email_Address==null) ? "":Record[0].Email_Address.ToString();
                    Address1_db.Text = (Record[0].Address1==null) ? "":Record[0].Address1.ToString();
                    Address2_db.Text = (Record[0].Address2==null) ? "":Record[0].Address2.ToString();
                    City_db.Text = (Record[0].City==null) ? "":Record[0].City.ToString();
                    State_db.Text = (Record[0].State==null) ? "":Record[0].State.ToString();
                    Country_db.Text = (Record[0].Country==null) ? "":Record[0].Country.ToString();
                    Cell_Phone_Number_db.Text = (Record[0].Cell_Phone_Number==null) ? "":Record[0].Cell_Phone_Number.ToString();
                    Office_Number_db.Text = (Record[0].Office_Number==null) ? "":Record[0].Office_Number.ToString();
                    Postal_Code_db.Text = (Record[0].Postal_Code==null) ? "":Record[0].Postal_Code.ToString();
                    Type_of_client_db.SelectedValue = Record[0].Type_Of_Client.ToString();//(Record[0].Type_Of_Client.ToString() == "INDV") ? 1 : 2;
                    Complaint_Channel_db.SelectedValue = Record[0].Complaint_Channel.ToString();

                    Category cat = Utility.fillCategory(Record[0].Complaint_Sub_Category_Code.ToString());

                    Complaint_Category_db.SelectedValue = cat.CategoryId;

                    Complaint_Category_Code_db.Items.Clear();
                    loadComplaintSubCategory(Complaint_Category_db.SelectedItem.Value);

                    Complaint_Category_Code_db.SelectedValue= Record[0].Complaint_Sub_Category_Code.ToString();

                    dtReceivedPicker.Text = Record[0].Date_Received.ToString();
                    Subject_Of_Complaint_db.Text = (Record[0].Subject_Of_Complaint==null) ? "":Record[0].Subject_Of_Complaint.ToString();
                    Complaint_Description_db.Text = (Record[0].Complaint_Description==null) ? "":Record[0].Complaint_Description.ToString();
                    Amount_In_Dispute_db.Text = (Record[0].Amount_In_Dispute==null) ? "":Record[0].Amount_In_Dispute.ToString();
                    Amount_Refunded_Petitioner_db.Text = (Record[0].Amount_Refunded_Petitioner==null) ? "":Record[0].Amount_Refunded_Petitioner.ToString();
                    Amount_Recovered_by_Bank_db.Text = (Record[0].Amount_Recovered_by_Bank==null) ? "":Record[0].Amount_Recovered_by_Bank.ToString();
                    Action_Taken_db.Text = (Record[0].Action_Taken==null) ? "":Record[0].Action_Taken.ToString();

                    status_of_complaint.SelectedValue = Record[0].Status_Of_Complaint.ToString();
                    if (Record[0].Status_Of_Complaint.ToString() == "CLOSED" || Record[0].Status_Of_Complaint.ToString() == "RESOLVED")
                    {                        
                        divDateClosed.Visible = true;
                        dtClosedPicker.Text = Record[0].Date_Closed.ToString();
                    }
                    else divDateClosed.Visible = false;

                    Remarks_by_Bank_db.Text = (Record[0].Remarks_by_Bank==null) ? "":Record[0].Remarks_by_Bank.ToString();
                 
                      
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", " showscreen('formlink');", true);       
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('System Error','System was unable to fetch record. Please try again later.','warning');", true);
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }

    protected void EntityDataSource1_Selected(object sender, EntityDataSourceSelectedEventArgs e)
    {
        countlbl.Text = e.TotalRowCount.ToString();
    }
 
    protected void exportbtn_Click(object sender, EventArgs e)
    {
        if(serverparameter.Value.Split(new char[] { '|' })[0].ToString()=="excel checked items" || serverparameter.Value.Split(new char[] { '|' })[0].ToString()=="excel")
        {
           exportexcel();
        }
     
        //if(serverparameter.Value.Split(new char[] { '|' })[0].ToString()=="csv checked items" || serverparameter.Value.Split(new char[] { '|' })[0].ToString()=="csv")
        //{
        //   exportcsv();
        //}
    }
       
    protected void exportexcel()
    {
        using (var context = new CCMSEntities())
        {
            var sb = new StringBuilder();
            var grid = new System.Web.UI.WebControls.GridView();
            grid.RowDataBound += exportgridview_RowDataBound;

            if (serverparameter.Value.Contains("|"))
            {
                List<string> ids = new List<string>();
                foreach (string rec_id in serverparameter.Value.Split(new char[] { '|' })[1].Split(new char[] { ',' }))
                {
                    if (rec_id != "")
                    {
                        ids.Add(rec_id);
                    }
                }

                var data = from it in context.SLAStatus_view
                           where ids.Contains(it.Tracking_Reference_Number)
                           select new
                           {
                               Tracking_Reference_Number= it.Tracking_Reference_Number,
                               Account_Branch_Name= it.Account_Branch_Name,
                               it.Name_Of_Consultant,
                               it.Type_Of_Client,
                               it.Name_Of_Complainant,
                               it.First_Name,
                               it.Middle_Name,
                               it.Last_Name,
                               it.Unique_Identification_No,
                               it.Account_Number,
                               it.Account_Type,
                               it.Account_Currency,
                               it.Address1,
                               it.Address2,
                               it.City,
                               it.State,
                               it.Country,
                               it.Postal_Code,
                               it.Cell_Phone_Number,
                               it.Office_Number,
                               it.Complaint_Channel,
                               it.Complaint_Location_Branch,
                               it.Email_Address,
                               it.Complaint_Category,
                               it.Complaint_Sub_Category_Code,
                               it.Subject_Of_Complaint,
                               it.Complaint_Description,
                               it.Date_Received,
                               it.Date_Closed,
                               it.Amount_In_Dispute,
                               it.Amount_Refunded_Petitioner,
                               it.Amount_Recovered_by_Bank,
                               it.Action_Taken,
                               it.Status_Of_Complaint,
                               it.Remarks_by_Bank,
                               it.Date_Created,
                               it.Created_By,
                               it.Supervisor_email,
                               it.Date_Modifiied,
                               it.Modified_By,
                               it.CBN_SLA_Limit_in_days,
                               it.Internal_Limit_in_days,
                               it.SLA_STATUS
                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from it in context.SLAStatus_view
                           select new
                           {
                               it.Tracking_Reference_Number,
                               it.Account_Branch_Name,
                               it.Name_Of_Consultant,
                               it.Type_Of_Client,
                               it.Name_Of_Complainant,
                               it.First_Name,
                               it.Middle_Name,
                               it.Last_Name,
                               it.Unique_Identification_No,
                               it.Account_Number,
                               it.Account_Type,
                               it.Account_Currency,
                               it.Address1,
                               it.Address2,
                               it.City,
                               it.State,
                               it.Country,
                               it.Postal_Code,
                               it.Cell_Phone_Number,
                               it.Office_Number,
                               it.Complaint_Channel,
                               it.Complaint_Location_Branch,
                               it.Email_Address,
                               it.Complaint_Category,
                               it.Complaint_Sub_Category_Code,
                               it.Subject_Of_Complaint,
                               it.Complaint_Description,
                               it.Date_Received,
                               it.Date_Closed,
                               it.Amount_In_Dispute,
                               it.Amount_Refunded_Petitioner,
                               it.Amount_Recovered_by_Bank,
                               it.Action_Taken,
                               it.Status_Of_Complaint,
                               it.Remarks_by_Bank,
                               it.Date_Created,
                               it.Created_By,
                               it.Supervisor_email,
                               it.Date_Modifiied,
                               it.Modified_By,
                               it.CBN_SLA_Limit_in_days,
                               it.Internal_Limit_in_days,
                               it.SLA_STATUS
                           };

                grid.DataSource = data.ToList();
            }
            grid.DataBind();
            Response.ClearContent();
            
	    Response.AddHeader("content-disposition", "attachment; filename=export.xls");
	    Response.ContentType = "application/vnd.ms-excel";   
            StringWriter sw = new StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();

        }
    }

    //protected void exportcsv()
    //{
    //    using (var context = new CCMSEntities())
    //    {

    //        List<string> ids = new List<string>();
    //        if (serverparameter.Value.Contains("|"))
    //        {
    //          foreach (string rec_id in serverparameter.Value.Split(new char[] { '|' })[1].Split(new char[] { ',' }))
    //          {
    //              if (rec_id != "")
    //              {
    //                  ids.Add(rec_id);
    //              }
    //          }
    //        }

    //        List<SLAStatus_view> reocrds =(ids.Count > 0) ? 
    //              (from d in context.SLAStatus_view where ids.Contains(d.Tracking_Reference_Number) select d).ToList():
    //              (from d in context.SLAStatus_view select d).ToList();

    //        MemoryStream ms = new MemoryStream();
    //        TextWriter tw = new StreamWriter(ms);

    //        tw.WriteLine("Account Number,Ref Number,Last Name,First Name,Middle Name");

    //        foreach (SLAStatus_view record in reocrds)
    //        {
    //            tw.WriteLine(record.Tracking_Reference_Number.ToString().Replace(",", "")+","+record.Name_Of_Complainant.ToString().Replace(",", "")
    //                +","+record.Subject_Of_Complaint.ToString().Replace(",", "")+","+record.Complaint_Description.ToString().Replace(",", "")+","+record.Status_Of_Complaint.ToString().Replace(",", "")
    //                + "," + record.Date_Received.ToString().Replace(",", "") + "," + record.Date_Created.ToString().Replace(",", "") + "," + record.Created_By.ToString().Replace(",", "")
    //                + "," + record.Internal_Limit.ToString().Replace(",", "") + "," + record.SLA_STATUS.ToString().Replace(",", ""));
    //        }

    //        tw.Flush();
    //        byte[] bytes = ms.ToArray();
    //        ms.Close();

    //        Response.Clear();
    //        Response.ContentType = "application/force-download";
    //        Response.AddHeader("content-disposition", "attachment;    filename=export.csv");
    //        Response.BinaryWrite(bytes);
    //        Response.End();
    //    }
    //}

    private void exportgridview_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //format gridview values for export
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            foreach (TableCell cell in e.Row.Cells)
            {
                if (cell.Text.Trim().Length > 1 && cell.Text.Trim().StartsWith("0"))
                {
                    cell.Style.Add("mso-number-format", "\\@");
                }
            }
        }
    }
   
   
    private string matchAccountType(string accountType)
    {      
        if (accountType.Contains("CURRENT"))
        {
            accountType = "CURRENT";    
        }
        else if (accountType.Contains("SAVINGS"))
        {
            accountType = "SAVINGS";
        }
        else if (accountType.Contains("SAVINGS"))
        {
            accountType = "SAVINGS";
        }
        else if (accountType.Contains("LOAN"))
        {
            accountType = "LOAN";
        }
        else if (accountType.Contains("DOMICILIARY"))
        {
            accountType = "DOMICILIARY";
        }
        else 
            accountType = "OTHER";

        return accountType;
    }

    private string matchAccountCurrency(string accountCurrency)
    {
        if (accountCurrency == "NAIRA")
        {
            accountCurrency = "NGN";
        }
        else if (accountCurrency == "US DOLLAR")
        {
            accountCurrency = "USD";
        }
        else if (accountCurrency == "BRITISH POUND")
        {
            accountCurrency = "GBP";
        }
        else if (accountCurrency == "EURO")
        {
            accountCurrency = "EUR";
        }

        return accountCurrency;
    }
}