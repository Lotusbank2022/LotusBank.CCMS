using EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using OfficeOpenXml;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SunTrustUSSD.Utilities;

public partial class users : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Thread.Sleep(1000);
        if (!IsPostBack)
        {
            search();
            bind_Group_Id_db();
            loadUserTeamEmail();

        }
    }


    protected void bind_Group_Id_db()
    {
        Group_Id_db.Items.Clear();
        Group_Id_db.Items.Add("");
        using (var context = new CCMSEntities())
        {
            List<UserGroup> records = (from d in context.UserGroup select d).ToList();
            foreach (UserGroup record in records)
            {
                Group_Id_db.Items.Add(new ListItem(record.GroupName.ToString(), record.Id.ToString()));
            }
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
        try
        {
            EntityDataSource1.Where = " it.[Login] like '%" + searchtxt.Text + "%' || it.[GroupName] like '%" + searchtxt.Text + "%'";
            GridView1.DataBind();
        }
        catch { }
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
        if (Login_db.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Missing Information', 'Login is missing', 'error'); ", true);
            return;
        }

        if (Group_Id_db.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Missing Information', 'Group is missing', 'error'); ", true);
            return;
        }

        if (UserType_db.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Missing Information', 'User Type is missing', 'error'); ", true);
            return;
        }

        if (Status_db.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Missing Information', 'Status is missing', 'error'); ", true);
            return;
        }



        Utility.SaveResponse response = saverecord(recordid.Value.ToString(), Login_db.Text, Group_Id_db.Text, UserType_db.Text, Status_db.Text,
            Convert.ToString(drpTeam_email.SelectedItem), Convert.ToString(drpTeam_email.SelectedValue));
        if (response.code == "00")
        {
            search();

            Login_db.Text = "";
            Group_Id_db.Text = "";
            UserType_db.Text = "";
            Status_db.Text = "";

            recordid.Value = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Success','" + response.message + "','info'); showscreen('browselink');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','" + response.message + "','warning');", true);
        }
    }




    protected Utility.SaveResponse saverecord(string recid, string login, string group_id, string usertype, string status, string teamEmail, string CategoryName)
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                if (recid == "")
                {
                    //prevent saving depublicate record

                    var newrecord = new Users
                    {
                        Login = login,
                        Group_Id = int.Parse(group_id),
                        UserType = usertype,
                        Status = status,                        TeamEmail = teamEmail,                        CategoryName = CategoryName,
                    };
                    context.Users.Add(newrecord);
                    context.SaveChanges();

                     return new Utility.SaveResponse("00", "Record was saved successfully");
                }
                else
                {
                    int id = int.Parse(recid);
                    var existingrecord = context.Users.Where(rec => (rec.Id == id)).ToList();
                    existingrecord.ForEach(a => { a.Login = login; a.Group_Id = int.Parse(group_id); a.UserType = usertype; a.Status = status;
                        a.TeamEmail = teamEmail; a.CategoryName = CategoryName; });
                    context.SaveChanges();
 
                    return new Utility.SaveResponse("00", "Record was updated successfully");
                }
            }
        }
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
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
                List<Users> Record = (from d in context.Users where d.Id == id select d).ToList();

                if (Record.Count > 0)
                {
                    recordid.Value = Record[0].Id.ToString();
                    Login_db.Text = (Record[0].Login == null) ? "" : Record[0].Login.ToString();
                    Group_Id_db.Text = (Record[0].Group_Id == null) ? "" : Record[0].Group_Id.ToString();
                    UserType_db.Text = (Record[0].UserType == null) ? "" : Record[0].UserType.ToString();
                    Status_db.Text = (Record[0].Status == null) ? "" : Record[0].Status.ToString();
                    drpTeam_email.SelectedValue = (Record[0].CategoryName == null ? "0" : Record[0].CategoryName.ToString());


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

                var data = from s in context.User_View
                           where ids.Contains(s.Id)
                           select new
                           {
                               s.Id,
                               s.Login,
                               s.GroupName,
                               s.UserStatus,                               s.CategoryName,
                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from s in context.User_View
                           select new
                           {
                               s.Id,
                               s.Login,
                               s.GroupName,
                               s.UserStatus,                               s.CategoryName,
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

            List<User_View> reocrds = (ids.Count > 0) ?
                  (from d in context.User_View where ids.Contains(d.Id) select d).ToList() :
                  (from d in context.User_View select d).ToList();

            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);

            tw.WriteLine("Login,User Group,User Status");

            foreach (User_View record in reocrds)
            {
                tw.WriteLine(record.Login.ToString().Replace(",", "") + "," + record.GroupName.ToString().Replace(",", "") + "," + record.UserStatus.ToString().Replace(",", ""));
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

    private void loadUserTeamEmail()
    {
        drpTeam_email.Items.Add(new ListItem("--Select--", "0"));
        try
        {
            using (var dbContext = new EF.CCMSEntities())
            {
                var query = (from d in dbContext.ComplaintsCategory
                             select new
                             {
                                 d.Notify,
                                 d.CategoryName
                             }).ToList();

                //.SqlQuery("select CategoryId, Notify from ComplaintsCategory");
                //.Where(d => d.CategoryId.ToString() == category).ToList();
                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        drpTeam_email.Items.Add(new ListItem(item.Notify.Trim(), item.CategoryName.ToString()));
                    }
                }

            }
        }
        catch (Exception ex) 
        {
            ErrHandler.WriteError("An error occurred when loading the email - " + ex.Message + " || " + ex.StackTrace, "");
        }
    }




}