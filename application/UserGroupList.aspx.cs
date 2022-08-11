using EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

public partial class UserGroupList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Thread.Sleep(1000);
        if (!IsPostBack)
        {
            search();
            bind_defaultpage_db();

        }
    }


    protected void bind_defaultpage_db()
    {
        defaultpage_db.Items.Clear();
        defaultpage_db.Items.Add("");
        using (var context = new CCMSEntities())
        {
            List<ApplicationResources> records = (from d in context.ApplicationResources select d).ToList();
            foreach (ApplicationResources record in records)
            {
                defaultpage_db.Items.Add(new ListItem(record.ResouceName.ToString(), record.ResouceName.ToString()));
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
            GridView1.DataBind();
        }
        catch { }
    }


    protected void search()
    {
        try
        {
            EntityDataSource1.Where = " it.[GroupName] like '%" + searchtxt.Text + "%'";
            GridView1.DataBind();
        }
        catch { }
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
        //perform validation here

        Utility.SaveResponse response = saverecord(recordid.Value.ToString(), GroupName_db.Text, defaultpage_db.Text, Status_db.Text);
        if (response.code == "00")
        {
            search();

            GroupName_db.Text = "";
            defaultpage_db.Text = "";
            Status_db.Text = "";

            recordid.Value = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Success','" + response.message + "','info'); showscreen('browselink');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','" + response.message + "','warning');", true);
        }
    }




    protected Utility.SaveResponse saverecord(string recid, string groupname, string defaultpage, string status)
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                if (recid == "")
                {
                    //prevent depublicate record

                    var newrecord = new UserGroup
                    {
                        GroupName = groupname,
                        defaultpage = defaultpage,
                        Status = status,
                    };
                    context.UserGroup.Add(newrecord);
                    context.SaveChanges();
                    return new Utility.SaveResponse("00", "Record was saved successfully");
                }
                else
                {
                    int id = int.Parse(recid);
                    var existingrecord = context.UserGroup.Where(rec => (rec.Id == id)).ToList();
                    existingrecord.ForEach(a => { a.GroupName = groupname; a.defaultpage = defaultpage; a.Status = status; });
                    context.SaveChanges();
                    return new Utility.SaveResponse("00", "Record was updated successfully");
                }
            }
        }
        catch (Exception ex)
        {
            return new Utility.SaveResponse("-1", "An unexpected error occured. Please try again later");

            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }


    public void getdetails()
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                int id = int.Parse(serverparameter.Value.ToString());
                List<UserGroup> Record = (from d in context.UserGroup where d.Id == id select d).ToList();

                if (Record.Count > 0)
                {
                    recordid.Value = Record[0].Id.ToString();
                    GroupName_db.Text = Record[0].GroupName.ToString();
                    defaultpage_db.Text = Record[0].defaultpage.ToString();
                    Status_db.Text = Record[0].Status.ToString();


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
        using (var context = new CCMSEntities())
        {
            var sb = new StringBuilder();
            var grid = new System.Web.UI.WebControls.GridView();
            grid.RowDataBound += Grid_RowDataBound;

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

                var data = from s in context.UserGroup
                           where ids.Contains(s.Id)
                           select new
                           {
                               s.Id,
                               s.GroupName,
                               s.Status,
                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from s in context.UserGroup
                           select new
                           {
                               s.Id,
                               s.GroupName,
                               s.Status,
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


    private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
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
}