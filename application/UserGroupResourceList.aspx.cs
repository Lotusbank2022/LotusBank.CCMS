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

public partial class UserGroupResourceList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Thread.Sleep(1000);
        if (!IsPostBack)
        {
            search();
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
        catch(Exception ex) { string msg = ex.Message; }
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
        try
        {
            using (var context = new CCMSEntities())
            {
                //delete all previously associated resources               
                int id = int.Parse(recordid.Value);
                context.GroupResources.RemoveRange(context.GroupResources.Where(x => x.Group_Id == id));
                context.SaveChanges();

                foreach (GridViewRow row in resoucesgrid.Rows)
                {
                    if (((CheckBox)row.Cells[0].FindControl("checkbox")).Checked)
                    {
                        var newrecord = new GroupResources
                        {
                            Group_Id = id,
                            ApplicationResource_Id = int.Parse(row.Cells[1].Text),
                        };
                        context.GroupResources.Add(newrecord);
                        context.SaveChanges();
                    }
                }

                search();
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Success','Record saved!','info'); showscreen('browselink');", true);
            }
        }
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Unexpected error occured','warning');", true);
        }
    }
     
    public void getdetails()
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                int id = int.Parse(serverparameter.Value.ToString());
                List<GroupResources_View> Record = (from d in context.GroupResources_View where d.Id == id select d).ToList();

                if (Record.Count > 0)
                {                    
                    recordid.Value = Record[0].Id.ToString();
                    groupname_db.Text = Record[0].GroupName;
                    foreach (GridViewRow gridrow in resoucesgrid.Rows)
                    {
                        ((CheckBox)gridrow.Cells[0].FindControl("checkbox")).Checked = false;

                        foreach (GroupResources_View row in Record)
                        {

                            if (gridrow.Cells[2].Text.ToLower() == row.ResouceName.ToString().ToLower())
                            {
                                ((CheckBox)gridrow.Cells[0].FindControl("checkbox")).Checked = true;
                            }
                        }
                    }
                 }
                else
                {
                     recordid.Value = id.ToString();
                     groupname_db.Text = (from d in context.UserGroup where d.Id == id select d).ToList()[0].GroupName;
                    foreach (GridViewRow gridrow in resoucesgrid.Rows)
                    {
                        ((CheckBox)gridrow.Cells[0].FindControl("checkbox")).Checked = false; 
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", " showscreen('formlink');", true);

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

                var data = from s in context.ApplicationResources
                           where ids.Contains(s.Id)
                           select new
                           {
                               s.Id,
                               s.ResouceName,                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from s in context.ApplicationResources
                           select new
                           {
                               s.Id,
                               s.ResouceName,                           };

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