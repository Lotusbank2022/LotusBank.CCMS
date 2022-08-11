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

public partial class ErrorLogPage : System.Web.UI.Page
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
            EntityDataSource1.Where = " it.[Method] like '%" + searchtxt.Text + "%' || it.[OtherInfo] like '%" + searchtxt.Text + "%' || it.[Message] like '%" + searchtxt.Text + "%'";
            GridView1.DataBind();
        }
        catch { }
    }


    protected void view()
    {
        getdetails();
    }


    protected void EntityDataSource1_Selected(object sender, EntityDataSourceSelectedEventArgs e)
    {
        countlbl.Text = e.TotalRowCount.ToString();
    }


    public void getdetails()
    {
        try
        {
            using (var context = new CCMSEntities())
            {
                int id = int.Parse(serverparameter.Value.ToString());
                List<ErrorLog> Record = (from d in context.ErrorLog where d.Id == id select d).ToList();

                if (Record.Count > 0)
                {
                    recordid.Value = Record[0].Id.ToString();
                    Method_db.Text = Record[0].Method.ToString();
                    LineNumber_db.Text = Record[0].LineNumber.ToString();
                    Message_db.Text = Record[0].Message.ToString();
                    OtherInfo_db.Text = Record[0].OtherInfo.ToString(); 

                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", " showscreen('formlink');", true);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('System Error','System was unable to fetch record. Please try again later.','warning');", true);             
        }
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

                var data = from s in context.ErrorLog
                           where ids.Contains(s.Id)
                           select new
                           {
                               // You can choose column name according your need  
                               s.Id,
                               s.Method,
                               s.LineNumber,
                               s.Message,
                               s.OtherInfo,
                           };

                grid.DataSource = data.ToList();
            }
            else
            {
                var data = from s in context.ErrorLog
                           select new
                           {
                               // You can choose column name according your need  
                               s.Id,
                               s.Method,
                               s.LineNumber,
                               s.Message,
                               s.OtherInfo,
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