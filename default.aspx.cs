using EF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _default : System.Web.UI.Page
{
    //public string thisyear = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            thisyear.Text = DateTime.Now.Year.ToString();

            switch (Request.QueryString["option"].ToString())
            {
                case "sessionexpired": break;

                case "logout": break;
            }
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }
        catch { }
    }

    protected void signinbtn_Click(object sender, EventArgs e)
    {
        try
        {
            //validate user credential on iportal
            SunTrustProxy.validationResponse response = SunTrustProxy.ValidateUser(useridtxt.Text, passwordtxt.Text);

            if (response == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Invalid Credential','error');", true);
                return;
            }

            if (response.responseCode != "00")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','" + response.responseDescription + "','error');", true);
                return;
            }

            //get staff details
            SunTrustProxy.Staff userinfo = SunTrustProxy.getStaff(response.staffId)[0];

            //get Supervisor details
            SunTrustProxy.Staff supervisor_info = SunTrustProxy.getStaff(response.supervisorStaffId)[0];

            Session["SupervisorEmail"] = supervisor_info.Email;

            using (var context = new EF.CCMSEntities())
            {
                //get all staff current user is relieving
                List<SunTrustProxy.Staff> AllRelievedStaff = SunTrustProxy.getAllRelievedStaff(response.staffId);
                if (AllRelievedStaff.Count > 0)
                {
                    //get list of all relieved staff that are profiled on application
                    ArrayList ProfiledStaff = new ArrayList();

                    foreach (SunTrustProxy.Staff staff in AllRelievedStaff)
                    {
                        if (userIsProfiled(staff.Email) == true)
                        {
                            ProfiledStaff.Add(staff);
                        }
                    }


                    if (ProfiledStaff.Count > 0)
                    {
                        //add current user to list of profiled staff if current user is profiled on application
                        if (userIsProfiled(response.email))
                        {
                            ProfiledStaff.Add(SunTrustProxy.getStaff(response.staffId)[0]);
                        }

                        //add list of profiled staff to session
                        Session.Add("profiledstaff", ProfiledStaff);

                        //save information of current user to session for validation when desired profile is selected
                        Session.Add("reliever_staffid", response.staffId);

                        //display list of profiled staff for current user to select
                        string profileshtml = "<table style=\"width:100%;\"><tr><td colspan=\"2\"><hr></td></tr>";
                        foreach (SunTrustProxy.Staff staff in ProfiledStaff)
                        {
                            staff.PhotoPath = (staff.PhotoPath.Replace(" ", "") == "" ? "application/img/defaultuserimage.png" : staff.PhotoPath);
                            profileshtml += "<tr style=\" cursor:pointer;\" onclick=\"signinprofile('" + staff.StaffId + "'); \"><td><img alt=\"image\" style=\"width: 70px; height: 70px; \" class=\"img-circle\" src=\"" + staff.PhotoPath + "\"></td> <td><div>" + staff.FirstName + " " + staff.LastName + "</div></tr><tr><td colspan=\"2\"><hr></td></tr>";
                        }
                        profileshtml += "</table>";
                        profilelist.InnerHtml = profileshtml;

                        signin_div.Visible = false;
                        select_profile.Visible = true;
                    }
                    else
                    {
                        //this code runs when user is relieving someone but the relieved staff is not profiled on current application
                        if (SignInUser(userinfo, supervisor_info))
                        {
                            RedirectUser();
                        }
                    }
                }
                else
                {
                    //this code runs when user is not relieving anyone
                    if (SignInUser(userinfo, supervisor_info))
                    {
                        RedirectUser();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Unable to login','error');", true);
            return;
        }
    }

    protected bool userIsProfiled(string login_or_email)
    {
        string login = login_or_email.Split(new char[] { '@' })[0].ToString().Trim();

        using (var context = new EF.CCMSEntities())
        {
            //confirm user exist in local db
            if ((from d in context.User_View where d.Login == login_or_email || d.Login == login select d).ToList().Count > 0)
                return true;
            else
                return false;
        }
    }

    protected bool SignInUser(SunTrustProxy.Staff staffinfo, SunTrustProxy.Staff supervisorInfo)
    {
        bool status = true;
        string login = staffinfo.Email.Split(new char[] { '@' })[0].ToString().Trim();
        try
        {

            using (var context = new EF.CCMSEntities())
            {
                //confirm user exist in local db
                List<User_View> found_user = (from d in context.User_View where d.Login == login select d).ToList();
                if (found_user.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Access denied','error');", true);
                    return false;
                }

                if (found_user[0].UserStatus != Utility.UserStatus.ACTIVE)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','User account has been disabled','error');", true);
                    return false;
                }

                if (found_user[0].GroupStatus != Utility.UserStatus.ACTIVE)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Group account has been disabled','error');", true);
                    return false;
                }

                CurrentUser CurrentUser = new CurrentUser();
                CurrentUser.isreliever = false;
                CurrentUser.userid = login;
                CurrentUser.staffid = staffinfo.StaffId;
                CurrentUser.branch = staffinfo.Branch;
                CurrentUser.email = staffinfo.Email;
                CurrentUser.photo = staffinfo.PhotoPath;
                CurrentUser.groupid = found_user[0].Group_Id.ToString();
                CurrentUser.groupname = found_user[0].GroupName;
                CurrentUser.lastlogindate = found_user[0].LastLoginDate.ToString();


                //set reliever information same as current user information 
                CurrentUser.relievedstaff_email = staffinfo.Email;
                CurrentUser.relievedstaff_userid = login;
                CurrentUser.relievedstaff_staffid = staffinfo.StaffId;


                //save user and relieved staff information is session
                Session.Add("user", CurrentUser);


                //update last login date
                var update_user = context.Users.Where(rec => (rec.Login == login)).ToList();
                update_user.ForEach(a => { a.LastLoginDate = DateTime.Now; });
                context.SaveChanges();

                LoginModel loginmodel = new LoginModel();
                Session.Add("userid", login);
                Session.Add("userName", staffinfo.FirstName + " " + staffinfo.LastName);
                Session.Add("groupid", found_user[0].Group_Id);

                Session.Add("usergroupname", found_user[0].GroupName);
                Session.Add("lastlogindate", found_user[0].LastLoginDate);

                Session["BranchCode"] = staffinfo.Branch;
                Session["TellerID"] = login;
                Session["AuthorizerID"] = (staffinfo.SupervisorStaffId == null) ? "" : staffinfo.SupervisorStaffId;

                Session["SupervisorEmail"] = supervisorInfo.Email;
                Session["BranchName"] = staffinfo.BranchName;

                Session["StaffId"] = staffinfo.StaffId;

                Session["CategoryName"] = found_user[0].CategoryName;
                Session["GroupName"] = found_user[0].GroupName;

                string ip = Request.ServerVariables["REMOTE_ADDR"].ToString();

                loginmodel.username = login;
                loginmodel.password = "";
                loginmodel.branchid = staffinfo.Branch;
                loginmodel.branchname = staffinfo.BranchName;
                loginmodel.userid = login;
                loginmodel.userrole = staffinfo.JobCode;
                loginmodel.IP = ip;

                Session["SessionID"] = Session.SessionID;

                clsUser oUser = new clsUser();
                if (oUser.insertUser(Session["TellerID"].ToString(), Session["SessionID"].ToString(), (string)Session["AuthorizerID"], (string)Session["SupervisorEmail"].ToString()))
                {
                    if (oUser.insertUserLog(loginmodel))
                    {
                        Session["UserSerialNo"] = oUser.UserSerialNo;

                        //Response.Redirect("application/payment.aspx",true);

                        Response.Redirect("application/" + found_user[0].defaultpage);
                    }
                    else
                    {
                        status = false;
                    }
                }
                else
                {
                    status = false;
                }

                //return true;
            }
        }
        catch (Exception ex)
        {
            status = false;

            Utility.LogError(
                        new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
                        ex.ToString(),
                        new StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(),
                        new StackTrace(ex, true).GetFrame(0).GetFileName()
                    );

        }
        return status;
    }

    protected bool SignInReliever(SunTrustProxy.Staff staffinfo, SunTrustProxy.Staff relieved_user)
    {
        string login = relieved_user.Email.Split(new char[] { '@' })[0].ToString().Trim();
        bool status = true;
        try
        {

            using (var context = new EF.CCMSEntities())
            {
                //confirm user exist in local db
                List<User_View> relieverstaff_profile = (from d in context.User_View where d.Login == login select d).ToList();
                if (relieverstaff_profile.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Selected staff is not profiled on this application','error');", true);
                    return false;
                }

                if (relieverstaff_profile[0].UserStatus != Utility.UserStatus.ACTIVE)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Selected staff profile has been disabled','error');", true);
                    return false;
                }

                if (relieverstaff_profile[0].GroupStatus != Utility.UserStatus.ACTIVE)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Selected staff profile group has been disabled','error');", true);
                    return false;
                }

                CurrentUser CurrentUser = new CurrentUser();
                CurrentUser.isreliever = true;
                CurrentUser.userid = staffinfo.Email.Split(new char[] { '@' })[0].ToString().Trim();
                CurrentUser.staffid = staffinfo.StaffId;
                CurrentUser.photo = staffinfo.PhotoPath;
                CurrentUser.email = staffinfo.Email;

                //make current user inherit profile right of staff been relieved
                // CurrentUser.usertype = relieverstaff_profile[0].UserType;
                CurrentUser.branch = relieved_user.Branch;
                CurrentUser.groupid = relieverstaff_profile[0].Group_Id.ToString();
                CurrentUser.groupname = relieverstaff_profile[0].GroupName;
                CurrentUser.lastlogindate = relieverstaff_profile[0].LastLoginDate.ToString();

                //set relieving staff info
                CurrentUser.relievedstaff_email = relieved_user.Email;
                CurrentUser.relievedstaff_userid = relieved_user.Email.Split(new char[] { '@' })[0].ToString().Trim();
                CurrentUser.relievedstaff_staffid = relieved_user.StaffId;

                //save user and relieved staff information is session
                Session.Add("user", CurrentUser);

                LoginModel loginmodel = new LoginModel();
                Session.Add("userid", login);
                Session.Add("userName", staffinfo.FirstName + " " + staffinfo.LastName);
                Session.Add("groupid", relieverstaff_profile[0].Group_Id);

                Session.Add("usergroupname", relieverstaff_profile[0].GroupName);
                Session.Add("lastlogindate", relieverstaff_profile[0].LastLoginDate);

                Session["BranchCode"] = staffinfo.Branch;
                Session["TellerID"] = login;
                Session["AuthorizerID"] = (staffinfo.SupervisorStaffId == null) ? "" : staffinfo.SupervisorStaffId;
                Session["BranchName"] = staffinfo.BranchName;

                Session["StaffId"] = staffinfo.StaffId;
                string ip = Request.ServerVariables["REMOTE_ADDR"].ToString();

                loginmodel.username = login;
                loginmodel.password = "";
                loginmodel.branchid = staffinfo.Branch;
                loginmodel.branchname = staffinfo.BranchName;
                loginmodel.userid = login;
                loginmodel.userrole = staffinfo.JobCode;
                loginmodel.IP = ip;

                Session["SessionID"] = Session.SessionID;

                clsUser oUser = new clsUser();
                if (oUser.insertUser(Session["TellerID"].ToString(), Session["SessionID"].ToString(), (string)Session["AuthorizerID"], (string)Session["SupervisorEmail"].ToString()))
                {
                    if (oUser.insertUserLog(loginmodel))
                    {
                        Session["UserSerialNo"] = oUser.UserSerialNo;

                        //Response.Redirect("application/payment.aspx",true);

                        Response.Redirect("application/" + relieverstaff_profile[0].defaultpage);
                    }
                    else
                    {
                        status = false;
                    }
                }
                else
                {
                    status = false;
                }
                //return true;
            }
        }
        catch (Exception ex)
        {
            status = false;

            Utility.LogError(
                        new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
                        ex.ToString(),
                        new StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(),
                        new StackTrace(ex, true).GetFrame(0).GetFileName()
                    );
        }
        return status;
    }

    protected void RedirectUser()
    {
        //redirect user application
        string redirectpage = "";
        string recordid = "";
        try
        {
            if (Request.QueryString["processpage"].ToString() != "")
            {
                redirectpage = Request.QueryString["processpage"].ToString();
            }
        }
        catch { }

        try
        {
            if (Request.QueryString["recordid"].ToString() != "")
            {
                recordid = "?recordid=" + Request.QueryString["recordid"].ToString();
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

        if (redirectpage != "")
        {
            Response.Redirect("application/" + redirectpage + recordid);
            return;
        }

        Response.Redirect("application/home.aspx");
    }

    protected void signinwithprofilebtn_Click(object sender, EventArgs e)
    {
        try
        {
            //ensure selected profile exist in profile list of profiles stored in session
            ArrayList ProfiledStaff = (ArrayList)Session["profiledstaff"];

            foreach (SunTrustProxy.Staff relievedstaff in ProfiledStaff)
            {
                if (relievedstaff.StaffId == selectedprofile.Value)
                {
                    //sign in reliever along with current user
                    string reliever_staffid = Session["reliever_staffid"].ToString();
                    SignInReliever(SunTrustProxy.getStaff(reliever_staffid)[0], SunTrustProxy.getStaff(relievedstaff.StaffId)[0]);

                    //redirect user
                    RedirectUser();

                    return;
                }
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
            //display error
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "showmsg('Error','Invalid profile selected','error');", true);
            return;
        }
    }

}