using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public partial class application_utility : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //[WebMethod]
    //public static Utility.ajaxresponse getaccountname(string accountnumber)
    //{
    //    try
    //    {
    //        SunTrustProxy.AccountDetails customerinfo = SunTrustProxy.GetAccountDetailsByAccountNumber(accountnumber, Utility.channelcode(), Utility.SunwareUrl());
    //        if (customerinfo.Nuban == "")
    //        {
    //            Utility.ajaxresponse response = new Utility.ajaxresponse();
    //            response.responsecode = "-1";
    //            response.responsemessage = "INVALID ACCOUNT";
    //            return response;
    //        }
    //        else
    //        {
    //            Utility.ajaxresponse response = new Utility.ajaxresponse();
    //            response.responsecode = "00";
    //            response.responsemessage = customerinfo.CustomerName;
    //            return response;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Utility.ajaxresponse response = new Utility.ajaxresponse();
    //        response.responsecode = "-1";
    //        response.responsemessage = "INVALID ACCOUNT";
    //        return response;
    //    }
    //}
}