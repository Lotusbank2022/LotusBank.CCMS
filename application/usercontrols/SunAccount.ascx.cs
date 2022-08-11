using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

public partial class application_SunAccount : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [Description("Text displayed in the textbox"), Category("Data")]
    public string AccountNumber
    {
        get { return acctno.Text; }
        set { acctno.Text = value; }
    }

    [Description("Text displayed as account name"), Category("Data")]
    public string AccountName
    {
        get { return acctname.Text; }
        set { acctname.Text = value; }
    }

    

    //create user control for staff, NIP,
}