using EF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pages_MasterPage : System.Web.UI.MasterPage
{
    CurrentUser CurrentUser;
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
        CurrentUser = (CurrentUser)Session["user"];
        Thread.Sleep(1000);
        try
        {
            if (!IsPostBack)
            {
                if (CurrentUser.photo != "")
                    userphoto.Src = CurrentUser.photo;

                //hide menu links
                foreach (Control htmlitem in sidemenu.Controls)
                {
                    if (htmlitem is System.Web.UI.HtmlControls.HtmlGenericControl)
                    {
                        var item = (System.Web.UI.HtmlControls.HtmlGenericControl)htmlitem;

                        if (item.TagName == "li")
                        {
                            item.Visible = false;
                        }
                    }
                }


                using (var context = new EF.CCMSEntities())
                {
                    int groupid = int.Parse(CurrentUser.groupid);
                    List<GroupResources_View> GroupResources = (from d in context.GroupResources_View where d.Id == groupid select d).ToList();
                    foreach (GroupResources_View resource in GroupResources)
                    {
                        try
                        {
                            sidemenu.FindControl(resource.ResouceName.Split(new char[] { '.' })[0].ToString()).Visible = true;
                        }
                        catch (Exception ex)
                        {
                            Utility.LogError(
                            new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
                            ex.ToString(),
                            new StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(),
                            new StackTrace(ex, true).GetFrame(0).GetFileName()
                        );
                        }
                    }
                }

                username.InnerText = CurrentUser.userid.ToUpper();
                usergrouplbl.InnerText = CurrentUser.groupname;
                lastloginlbl.InnerText = CurrentUser.lastlogindate;
            }
        }
        catch (Exception ex)
        {
            Utility.LogError(
                            new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
                            ex.ToString(),
                            new StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(),
                            new StackTrace(ex, true).GetFrame(0).GetFileName()
                        );
        }
    }


}
