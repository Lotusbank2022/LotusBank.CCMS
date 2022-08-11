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
using Newtonsoft.Json.Linq;
using System.Configuration;
using Newtonsoft.Json;
using SunTrustUSSD.Utilities;
using System.Data.Entity.Validation;

public partial class customercomplaint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Thread.Sleep(1000);
            clsUser oUser = new clsUser();
            if (Session["GroupName"].ToString().Trim().ToLower().Equals(ConfigurationManager.AppSettings["resolver"].ToString().Trim().ToLower()))
            {
                divNR.Visible = false;
            }
            else
            {
                divNR.Visible = true;
            }


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
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.ToString(), (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.Message);
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

            case "submitrecord": submitrecord(); break;
        }
        servermethod.Value = "";
    }

    protected void pagesize()
    {
        try
        {
            GridView1.PageSize = int.Parse(serverparameter.Value);
            search();
        }
        catch { }
    }


    protected void search()
    {
        //string resolved = "RESOLVED", closed = "CLOSED";
        try
        {
            if (Session["GroupName"].ToString().Trim().ToLower().Equals(ConfigurationManager.AppSettings["resolver"].ToString().Trim().ToLower()))
            {
                EntityDataSource1.Where = " (it.[Tracking_Reference_Number] like '%" + searchtxt.Text + "%' || it.[Last_Name] like '%" +
                    searchtxt.Text + "%' || it.[First_Name] like '%" + searchtxt.Text + "%' || it.[Middle_Name] like '%" + searchtxt.Text + "%') " +
                    "&& it.Complaint_Category = '" + (string)Session["CategoryName"] + "'";
                //&& it.[Status_Of_Complaint] != '" + resolved + 
                //"' && it.[Status_Of_Complaint] != '" + closed + "'";
                GridView1.DataBind();
            }
            else
            {
                EntityDataSource1.Where = " (it.[Tracking_Reference_Number] like '%" + searchtxt.Text + "%' || it.[Last_Name] like '%" +
                    searchtxt.Text + "%' || it.[First_Name] like '%" + searchtxt.Text + "%' || it.[Middle_Name] like '%" + searchtxt.Text + "%') " +
                    "&& it.Created_by = '" + (string)Session["TellerID"] + "'";
                GridView1.DataBind();
            }

        }
        catch (Exception e)
        {
            ErrHandler.WriteError("An Erro occurred when retrieving record from CustomerComplaints Table - " + e.Message, "");
        }
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
        savebtn.Style.Add("display", "none");
    }


    protected void edit()
    {
        getdetails();
        savebtn.Style.Add("display", "inline");
    }

    protected void submitrecord()
    {

        //Tracking_Reference
        if (string.IsNullOrEmpty(Tracking_Reference.Text))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Tracking Reference is required','error');", true);
            return;
        }

        if (Type_of_client_db.SelectedIndex == 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Select Type of Client','error');", true);
            return;
        }

        if (Complaint_Channel_db.SelectedIndex == 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Select Complaint Channel','error');", true);
            return;
        }

        if (Complaint_Category_db.SelectedIndex == 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Select Complaint Category','error');", true);
            return;
        }

        if (Complaint_Category_Code_db.SelectedIndex == 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Select Complaint Sub-Category','error');", true);
            return;
        }

        if (dtReceivedPicker.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Input Date Complaint was received','error');", true);
            return;
        }

        if (Subject_Of_Complaint_db.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Input Subject of Complaint','error');", true);
            return;
        }

        if (Complaint_Description_db.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Complaint Description is needed','error');", true);
            return;
        }

        if (Action_Taken_db.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Action Taken is needed','error');", true);
            return;
        }

        if (status_of_complaint.Text.Trim() == "--Select--")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Select Status of Complaint','error');", true);
            return;
        }

        if (string.IsNullOrEmpty(matchPhoneNumber(Cell_Phone_Number_db.Text.Trim())))
        {
            Cell_Phone_Number_db.ReadOnly = false;
            Office_Number_db.ReadOnly = false;

            Cell_Phone_Number_db.Text = "";
            Office_Number_db.Text = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Phone Number is needed. Pls conform to regular phone number pattern (e.g. 08012345678)','error');", true);
            return;
        }

        if (string.IsNullOrEmpty(matchPhoneNumber(Office_Number_db.Text.Trim())))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Office Number is needed. Pls conform to regular phone number pattern (e.g. 08012345678)','error');", true);
            return;
        }

        if (!string.IsNullOrEmpty(Email_Address_db.Text.Trim()))
        {
            if (!validateEmail(Email_Address_db.Text.Trim())) Email_Address_db.Text = "";
        }

        if (divDateClosed.Visible == true && dtClosedPicker.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Date Closed is needed','error');", true);
            return;
        }

        Utility.SaveResponse response = saverecord(
            recordid.Value.ToString(), Tracking_Reference.Text.Trim(), Account_Number_db.Text.Trim(), Name_Of_Complainant_db.Text.Trim(), Account_Branch_Name_db.Text.Trim(),
            First_Name_db.Text.Trim(), Middle_Name_db.Text.Trim(), Last_Name_db.Text.Trim(), Account_Type_db.Text.Trim(), Account_Currency_db.Text.Trim(),
            Unique_Identification_No_db.Text.Trim(), Email_Address_db.Text.Trim(), Address1_db.Text.Trim(), Address2_db.Text.Trim(), City_db.Text, State_db.Text.Trim(),
            Country_db.Text.Trim(), Cell_Phone_Number_db.Text.Trim(), Office_Number_db.Text.Trim(), Postal_Code_db.Text.Trim(), Type_of_client_db.SelectedValue,
            Complaint_Channel_db.Text.Trim(), Complaint_Category_db.SelectedItem.Text, Complaint_Category_Code_db.SelectedItem.Text, Complaint_Category_Code_db.SelectedItem.Value, Subject_Of_Complaint_db.Text.Trim(), Complaint_Description_db.Text.Trim(),
            Description_of_Resolution.Text.Trim(), Amount_In_Dispute_db.Text.Trim(), Amount_Refunded_Petitioner_db.Text.Trim(), Amount_Recovered_by_Bank_db.Text.Trim(), Action_Taken_db.Text.Trim(),
            status_of_complaint.SelectedValue, Remarks_by_Bank_db.Text.Trim()
            );

        if (response.code == "00")
        {
            search();

            Account_Number_db.Text = "";
            Name_Of_Complainant_db.Text = "";
            Account_Branch_Name_db.Text = "";
            First_Name_db.Text = "";
            Middle_Name_db.Text = "";
            Last_Name_db.Text = "";
            Account_Type_db.Text = "";
            Account_Currency_db.Text = "";
            Unique_Identification_No_db.Text = "";
            Email_Address_db.Text = "";
            Address1_db.Text = "";
            Address2_db.Text = "";
            City_db.Text = "";
            State_db.Text = "";
            Country_db.Text = "";
            Cell_Phone_Number_db.Text = "";
            Office_Number_db.Text = "";
            Postal_Code_db.Text = "";
            Complaint_Channel_db.Text = "";
            Complaint_Category_Code_db.SelectedIndex = 0;
            Subject_Of_Complaint_db.Text = "";
            Complaint_Description_db.Text = "";
            Description_of_Resolution.Text = "";
            Amount_In_Dispute_db.Text = "";
            Amount_Refunded_Petitioner_db.Text = "";
            Amount_Recovered_by_Bank_db.Text = "";
            Action_Taken_db.Text = "";
            Remarks_by_Bank_db.Text = "";

            recordid.Value = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Success','" + response.message + "','info'); showscreen('browselink');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','" + response.message + "','warning');", true);
        }
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
        else if (phoneNumber.Contains("-")) phoneNumber = phoneNumber + "";
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

    protected Utility.SaveResponse saverecord(string recid, string tracking_reference_number, string account_number, string name_of_complainant, string account_branch_name, string first_name, string middle_name,
            string last_name, string account_type, string account_currency, string unique_identification_no, string email_address, string address1, string address2,
            string city, string state, string country, string cell_phone_number, string office_number, string postal_code, string type_of_client, string complaint_channel, string Complaint_Category,
            string Complaint_Sub_Category, string complaint_category_code, string subject_of_complaint, string complaint_description, string Description_of_Resolution,
            string amount_in_dispute, string amount_refunded_petitioner, string amount_recovered_by_bank, string action_taken, string status_of_complaint, string remarks_by_bank)
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                //var dt_Received = Convert.ToDateTime(DateTime.ParseExact(dtReceivedPicker.Text, "M/d/yyyy", CultureInfo.CurrentCulture)).ToString("dd-MM-yyyy");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

                var dt_Closed = string.IsNullOrEmpty(dtClosedPicker.Text) ? "" : Convert.ToDateTime(dtClosedPicker.Text).ToString("dd-MM-yyyy");

                //var dt_Closed = string.IsNullOrEmpty(dtClosedPicker.Text) ? "" :
                //    //Convert.ToDateTime(DateTime.ParseExact(dtClosedPicker.Text, "M/d/yyyy", CultureInfo.CurrentCulture)).ToString("dd-MM-yyyy");
                //     DateTime.ParseExact(dtClosedPicker.Text, "M/d/yyyy", CultureInfo.GetCultureInfo("en-GB")).ToString("dd-MM-yyyy");

                if (recid == "")
                {
                    //prevent saving depublicate record
                    //string DateCreated = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                    DateTime dateCreated = DateTime.Now;
                    var newrecord = new CustomerComplaints
                    {
                        Tracking_Reference_Number = tracking_reference_number,
                        Account_Number = account_number,
                        Name_Of_Complainant = name_of_complainant,
                        Account_Branch_Name = account_branch_name,
                        Name_Of_Consultant = "NA",
                        First_Name = first_name,
                        Middle_Name = string.IsNullOrEmpty(middle_name) || first_name.Split(' ').Length > 1 ? "NA" : first_name.Split(' ').GetValue(1).ToString(),
                        Last_Name = last_name,
                        Account_Type = matchAccountType(account_type),
                        Account_Currency = matchAccountCurrency(account_currency),
                        Unique_Identification_No = unique_identification_no,
                        Email_Address = email_address,
                        Address1 = address1,
                        Address2 = string.IsNullOrEmpty(address2) ? "NA" : address2,
                        City = string.IsNullOrEmpty(city) ? "NA" : city,
                        State = string.IsNullOrEmpty(state) ? "NA" : state,
                        Country = "NG",//country,
                        Cell_Phone_Number = matchPhoneNumber(cell_phone_number),
                        Office_Number = matchPhoneNumber(office_number),
                        Postal_Code = "NA",
                        Type_Of_Client = type_of_client,
                        Complaint_Channel = complaint_channel,
                        Complaint_Category = Complaint_Category,
                        Complaint_Sub_Category = Complaint_Sub_Category,
                        Complaint_Sub_Category_Code = complaint_category_code,
                        Subject_Of_Complaint = subject_of_complaint,
                        Complaint_Description = complaint_description,
                        Description_of_Resolution = Description_of_Resolution,
                        Amount_In_Dispute = (amount_in_dispute == "") ? 0 : Decimal.Parse(amount_in_dispute),
                        Amount_Refunded_Petitioner = (amount_refunded_petitioner == "") ? 0 : Decimal.Parse(amount_refunded_petitioner),
                        Amount_Recovered_by_Bank = (amount_recovered_by_bank == "") ? 0 : Decimal.Parse(amount_recovered_by_bank),
                        Action_Taken = action_taken,
                        Status_Of_Complaint = status_of_complaint,
                        Remarks_by_Bank = remarks_by_bank,

                        Complaint_Location_Branch = "NA", //to-do
                        Complaint_Location_City = "NA",  //to-do


                        //Date_Received = Convert.ToDateTime(DateTime.ParseExact(dtReceivedPicker.Text, "M/d/yyyy", CultureInfo.CurrentCulture)).ToString("dd-MM-yyyy"),

                        Date_Received = Convert.ToDateTime(dtReceivedPicker.Text).ToString("dd-MM-yyyy"),//DateTime.ParseExact(dtReceivedPicker.Text, "M/d/yyyy", CultureInfo.GetCultureInfo("en-GB")).ToString("dd-MM-yyyy"),
                        Date_Closed = dt_Closed,
                        Date_Created = dateCreated,
                        //Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")),
                        Created_By = (string)Session["TellerID"]
                        //Modified_By = "",
                        //Date_Modifiied = ""
                    };
                    context.CustomerComplaints.Add(newrecord);
                    context.SaveChanges();

                    Utility.sendNotification(newrecord, Session["SupervisorEmail"].ToString(), Complaint_Category_db.SelectedItem.Text,
                        Complaint_Category_Code_db.SelectedItem.Text, complaint_category_code); //to-do

                    //@callnestedtablesave

                    return new Utility.SaveResponse("00", "Record was saved successfully");
                }
                else
                {
                    int id = int.Parse(recid);
                    var existingrecord = context.CustomerComplaints.Where(rec => (rec.ID == id)).ToList();
                    existingrecord.ForEach(
                        a =>
                        {
                            a.Tracking_Reference_Number = tracking_reference_number;
                            a.Account_Number = account_number;
                            a.Name_Of_Complainant = name_of_complainant;
                            a.Account_Branch_Name = account_branch_name;
                            a.Name_Of_Consultant = "NA";
                            a.First_Name = first_name;
                            a.Middle_Name = middle_name;
                            a.Last_Name = last_name;
                            a.Account_Type = account_type;
                            a.Account_Currency = account_currency;
                            a.Unique_Identification_No = unique_identification_no;
                            a.Email_Address = email_address;
                            a.Address1 = address1;
                            a.Address2 = address2;
                            a.City = city;
                            a.State = state;
                            a.Country = country;
                            a.Cell_Phone_Number = matchPhoneNumber(cell_phone_number);
                            a.Office_Number = matchPhoneNumber(office_number);
                            a.Postal_Code = postal_code;
                            a.Type_Of_Client = type_of_client;
                            a.Complaint_Channel = complaint_channel;
                            a.Complaint_Category = Complaint_Category;
                            a.Complaint_Sub_Category = Complaint_Sub_Category;
                            a.Complaint_Sub_Category_Code = complaint_category_code;
                            a.Subject_Of_Complaint = subject_of_complaint;
                            a.Complaint_Description = complaint_description;
                            a.Description_of_Resolution = Description_of_Resolution;
                            a.Amount_In_Dispute = (amount_in_dispute == "") ? 0 : Decimal.Parse(amount_in_dispute);
                            a.Amount_Refunded_Petitioner = (amount_refunded_petitioner == "") ? 0 : Decimal.Parse(amount_refunded_petitioner);
                            a.Amount_Recovered_by_Bank = (amount_recovered_by_bank == "") ? 0 : Decimal.Parse(amount_recovered_by_bank);
                            a.Action_Taken = action_taken;
                            a.Status_Of_Complaint = status_of_complaint;
                            a.Remarks_by_Bank = remarks_by_bank;

                            a.Complaint_Location_Branch = "NA"; //to-do
                            a.Complaint_Location_City = "NA";  //to-do
                            a.Date_Received = Convert.ToDateTime(dtReceivedPicker.Text).ToString("dd-MM-yyyy");
                            //DateTime.ParseExact(dtReceivedPicker.Text, "M/d/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"); //dtReceivedPicker.Text;
                            a.Date_Closed = dt_Closed; //string.IsNullOrEmpty(dtClosedPicker.Text) ? "" : Convert.ToDateTime(dtClosedPicker.Text).ToString("dd-MM-yyyy");
                            a.Modified_By = (string)Session["TellerID"];
                            a.Date_Modifiied = DateTime.Now;
                            //DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")); //2018-12-21 09:44:40.230
                        });
                    context.SaveChanges();  //to-do

                    //Utility.notifyOnModification(a, Session["SupervisorEmail"].ToString(), Complaint_Category_db.SelectedItem.Text, Complaint_Category_Code_db.SelectedItem.Text,
                    //    complaint_category_code);

                    //@callnestedtablesave

                    return new Utility.SaveResponse("00", "Record was updated successfully");
                }
            }
        }
        catch (DbEntityValidationException e)
        {
            foreach (var eve in e.EntityValidationErrors)
            {
                ErrHandler.WriteError("Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:",
                    "");
                foreach (var ve in eve.ValidationErrors)
                {
                    ErrHandler.WriteError("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage, "");
                }
            }
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), e.ToString(), (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + e.Message);

            return new Utility.SaveResponse("-1", "An unexpected error occured. Please try again later");
        }
    }

    public void getdetails()
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                int id = int.Parse(serverparameter.Value.ToString());
                List<CustomerComplaints> Record = (from d in context.CustomerComplaints where d.ID == id select d).ToList();

                if (Record.Count > 0)
                {
                    recordid.Value = Record[0].ID.ToString();
                    Tracking_Reference.Text = (Record[0].Tracking_Reference_Number == null) ? "" : Record[0].Tracking_Reference_Number.ToString();
                    Account_Number_db.Text = (Record[0].Account_Number == null) ? "" : Record[0].Account_Number.ToString();
                    Account_Number.Text = (Record[0].Account_Number == null) ? "" : Record[0].Account_Number.ToString();
                    Name_Of_Complainant_db.Text = (Record[0].Name_Of_Complainant == null) ? "" : Record[0].Name_Of_Complainant.ToString();
                    Account_Branch_Name_db.Text = (Record[0].Account_Branch_Name == null) ? "" : Record[0].Account_Branch_Name.ToString();
                    First_Name_db.Text = (Record[0].First_Name == null) ? "" : Record[0].First_Name.ToString();
                    Middle_Name_db.Text = (Record[0].Middle_Name == null) ? "" : Record[0].Middle_Name.ToString();
                    Last_Name_db.Text = (Record[0].Last_Name == null) ? "" : Record[0].Last_Name.ToString();
                    Account_Type_db.Text = (Record[0].Account_Type == null) ? "" : Record[0].Account_Type.ToString();
                    Account_Currency_db.Text = (Record[0].Account_Currency == null) ? "" : Record[0].Account_Currency.ToString();
                    Unique_Identification_No_db.Text = (Record[0].Unique_Identification_No == null) ? "" : Record[0].Unique_Identification_No.ToString();
                    Email_Address_db.Text = (Record[0].Email_Address == null) ? "" : Record[0].Email_Address.ToString();
                    Address1_db.Text = (Record[0].Address1 == null) ? "" : Record[0].Address1.ToString();
                    Address2_db.Text = (Record[0].Address2 == null) ? "" : Record[0].Address2.ToString();
                    City_db.Text = (Record[0].City == null) ? "" : Record[0].City.ToString();
                    State_db.Text = (Record[0].State == null) ? "" : Record[0].State.ToString();
                    Country_db.Text = (Record[0].Country == null) ? "" : Record[0].Country.ToString();
                    Cell_Phone_Number_db.Text = (Record[0].Cell_Phone_Number == null) ? "" : Record[0].Cell_Phone_Number.ToString();
                    Office_Number_db.Text = (Record[0].Office_Number == null) ? "" : Record[0].Office_Number.ToString();
                    Postal_Code_db.Text = (Record[0].Postal_Code == null) ? "" : Record[0].Postal_Code.ToString();
                    Type_of_client_db.SelectedValue = Record[0].Type_Of_Client.ToString();//(Record[0].Type_Of_Client.ToString() == "INDV") ? 1 : 2;
                    Complaint_Channel_db.SelectedValue = Record[0].Complaint_Channel.ToString();// (Record[0].Complaint_Channel==null) ? "":Record[0].Complaint_Channel.ToString();
                    //Complaint_Category_Code_db.Text = (Record[0].Complaint_Category_Code == null) ? "" : Record[0].Complaint_Category_Code.ToString();

                    Category cat = Utility.fillCategory(Record[0].Complaint_Sub_Category_Code.ToString());
                    //Complaint_Category_db.Items.Clear();
                    //loadComplaintCategory();

                    Complaint_Category_db.SelectedValue = cat.CategoryId;

                    Complaint_Category_Code_db.Items.Clear();
                    loadComplaintSubCategory(Complaint_Category_db.SelectedItem.Value);

                    Complaint_Category_Code_db.SelectedValue = Record[0].Complaint_Sub_Category_Code.ToString();

                    Description_of_Resolution.Text = string.IsNullOrEmpty(Record[0].Description_of_Resolution) ? "" : Record[0].Description_of_Resolution.ToString();

                    dtReceivedPicker.Text = Record[0].Date_Received.ToString();
                    Subject_Of_Complaint_db.Text = (Record[0].Subject_Of_Complaint == null) ? "" : Record[0].Subject_Of_Complaint.ToString();
                    Complaint_Description_db.Text = (Record[0].Complaint_Description == null) ? "" : Record[0].Complaint_Description.ToString();
                    Amount_In_Dispute_db.Text = (Record[0].Amount_In_Dispute == null) ? "" : Record[0].Amount_In_Dispute.ToString();
                    Amount_Refunded_Petitioner_db.Text = (Record[0].Amount_Refunded_Petitioner == null) ? "" : Record[0].Amount_Refunded_Petitioner.ToString();
                    Amount_Recovered_by_Bank_db.Text = (Record[0].Amount_Recovered_by_Bank == null) ? "" : Record[0].Amount_Recovered_by_Bank.ToString();
                    Action_Taken_db.Text = (Record[0].Action_Taken == null) ? "" : Record[0].Action_Taken.ToString();

                    status_of_complaint.SelectedValue = Record[0].Status_Of_Complaint.ToString();
                    if (Record[0].Status_Of_Complaint.ToString() == "CLOSED" || Record[0].Status_Of_Complaint.ToString() == "RESOLVED")
                    {
                        divDateClosed.Visible = true;
                        dtClosedPicker.Text = Record[0].Date_Closed.ToString();
                    }
                    else divDateClosed.Visible = false;

                    Remarks_by_Bank_db.Text = (Record[0].Remarks_by_Bank == null) ? "" : Record[0].Remarks_by_Bank.ToString();


                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", " showscreen('formlink');", true);
                }
            }
        }
        catch (DbEntityValidationException e)
        {
            foreach (var eve in e.EntityValidationErrors)
            {
                ErrHandler.WriteError("Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:",
                    "");
                foreach (var ve in eve.ValidationErrors)
                {
                    ErrHandler.WriteError("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage, "");
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('System Error','System was unable to fetch record. Please try again later.','warning');", true);
        }
        //catch (Exception ex)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('System Error','System was unable to fetch record. Please try again later.','warning');", true);
        //    Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.ToString(), (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.Message );
        //}
    }

    protected void EntityDataSource1_Selected(object sender, EntityDataSourceSelectedEventArgs e)
    {
        countlbl.Text = e.TotalRowCount.ToString();
    }

    protected void exportbtn_Click(object sender, EventArgs e)
    {
        if (serverparameter.Value.Split(new char[] { '|' })[0].ToString() == "excel checked items" || serverparameter.Value.Split(new char[] { '|' })[0].ToString() == "excel")
        {
            exportexcel();
        }

        if (serverparameter.Value.Split(new char[] { '|' })[0].ToString() == "csv checked items" || serverparameter.Value.Split(new char[] { '|' })[0].ToString() == "csv")
        {
            exportcsv();
        }
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
                List<int> ids = new List<int>();
                foreach (string rec_id in serverparameter.Value.Split(new char[] { '|' })[1].Split(new char[] { ',' }))
                {
                    if (rec_id != "")
                    {
                        ids.Add(int.Parse(rec_id));
                    }
                }

                var data = from s in context.CustomerComplaints
                           where ids.Contains(s.ID)
                           select new
                           {
                               s.ID,
                               s.Account_Number,
                               s.Tracking_Reference_Number,
                               s.Last_Name,
                               s.First_Name,
                               s.Middle_Name,

                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from s in context.CustomerComplaints
                           select new
                           {
                               s.ID,
                               s.Account_Number,
                               s.Tracking_Reference_Number,
                               s.Last_Name,
                               s.First_Name,
                               s.Middle_Name,
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

    protected void exportcsv()
    {
        using (var context = new CCMSEntities())
        {

            List<int> ids = new List<int>();
            if (serverparameter.Value.Contains("|"))
            {
                foreach (string rec_id in serverparameter.Value.Split(new char[] { '|' })[1].Split(new char[] { ',' }))
                {
                    if (rec_id != "")
                    {
                        ids.Add(int.Parse(rec_id));
                    }
                }
            }

            List<CustomerComplaints> reocrds = (ids.Count > 0) ?
                  (from d in context.CustomerComplaints where ids.Contains(d.ID) select d).ToList() :
                  (from d in context.CustomerComplaints select d).ToList();

            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);

            tw.WriteLine("Account Number,Ref Number,Last Name,First Name,Middle Name");

            foreach (CustomerComplaints record in reocrds)
            {
                tw.WriteLine(record.Account_Number.ToString().Replace(",", "") + "," + record.Tracking_Reference_Number.ToString().Replace(",", "") + "," + record.Last_Name.ToString().Replace(",", "") + "," + record.First_Name.ToString().Replace(",", "") + "," + record.Middle_Name.ToString().Replace(",", ""));
            }

            tw.Flush();
            byte[] bytes = ms.ToArray();
            ms.Close();

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment;    filename=export.csv");
            Response.BinaryWrite(bytes);
            Response.End();
        }
    }

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

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(Account_Number_db.Text) || Account_Number_db.Text.Length != 10)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Invalid Account Number','error');", true);
                return;
            }
            else
            {
                // SunTrustProxy.AccountDetails acctDetails = SunTrustProxy.getAccountBynumber(Account_Number_db.Text.Trim()).Items[0];

                //Information/EnquiryByNuban
                dynamic req = new JObject();
                req.Nuban = Account_Number_db.Text.Trim();
                string fullUrl = string.Concat(Convert.ToString(ConfigurationManager.AppSettings["sunware"]), Convert.ToString(ConfigurationManager.AppSettings["EquiryByNuban"]));

                dynamic regresp = Utility.ExecuteHttpRequest(req, fullUrl);

                SunTrustProxy.AccountEnquiryResponse acctDetails = JsonConvert.DeserializeObject<SunTrustProxy.AccountEnquiryResponse>(Convert.ToString(regresp));

                if (acctDetails.responseCode == "00")
                {

                    Tracking_Reference.Text = "SUN" + DateTime.Now.ToString("ddMMyyhhmmss");
                    Name_Of_Complainant_db.Text = acctDetails.Items[0].AccountName;
                    Account_Branch_Name_db.Text = acctDetails.Items[0].BranchCode;
                    First_Name_db.Text = acctDetails.Items[0].FirstName;
                    Middle_Name_db.Text = "";
                    Last_Name_db.Text = acctDetails.Items[0].LastName;
                    Account_Type_db.Text = acctDetails.Items[0].AccountType;
                    Account_Currency_db.Text = acctDetails.Items[0].Currency;
                    Unique_Identification_No_db.Text = (acctDetails.Items[0].Bvn == "") ? "NA" : acctDetails.Items[0].Bvn;
                    Email_Address_db.Text = acctDetails.Items[0].Email;
                    Address1_db.Text = acctDetails.Items[0].Address;
                    //City_db.Text=acctDetails
                    //State_db.Text
                    //Country_db.Text
                    Cell_Phone_Number_db.Text = acctDetails.Items[0].Phone;
                    Office_Number_db.Text = acctDetails.Items[0].Phone;
                    Account_Number.Text = Account_Number_db.Text.Trim();
                }

            }
        }
        catch (Exception ex)
        {
            ErrHandler.WriteError("btnSearch_Click Error occurred  - " + ex.Message + "||" + ex.InnerException, "");
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Error occurred while retrieving account details','error');", true);
            return;
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
            accountType = "OTHERS";

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