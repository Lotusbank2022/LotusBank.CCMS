<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        try
        {
            var page = System.IO.Path.GetFileName(Request.PhysicalPath).ToLower();
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            string resourcefolder = url.Split(new char[] { '/' })[4].ToString().ToLower();
            bool accessdenied = false;

            if (page != "" && page.ToLower().Contains(".aspx") && page != "accessdenied.aspx" && resourcefolder == "application")
            {
                try
                {
                    using (var context = new EF.CCMSEntities())
                    {
                        int groupid = int.Parse(Session["groupid"].ToString());
                        List<EF.GroupResources_View> groupresources = (from d in context.GroupResources_View where d.Id == groupid && d.ResouceName.ToLower() == page select d).ToList();
                        if (groupresources.Count == 0)
                        {
                            accessdenied = true;
                            Response.Redirect("accessdenied.aspx");
                        }
                    }
                }
                catch
                {
                    if (accessdenied == false)
                    { Response.Redirect("../default.aspx"); }
                }
            }
        }
        catch(Exception ex) { }
    }

</script>
