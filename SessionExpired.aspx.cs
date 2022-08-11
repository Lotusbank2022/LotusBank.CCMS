using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Collections;

public partial class SessionExpired : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string code = Request["err"].ToString().Trim();

            if (code == "timedout")
            {
                lblError.Text = "Your session has expired. Please relogin";
            }
            else if (code == "concurrentlogin")
            {
                lblError.Text = "You already have an existing session. Kindly log off that session";
            }
            else if (code == "exception")
            {
                lblError.Text = "An unexpected error occurred.";
            }
            else { }
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