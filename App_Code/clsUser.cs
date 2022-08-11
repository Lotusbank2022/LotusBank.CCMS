
using EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for clsUser
/// </summary>
public class clsUser
{
    public string UserSerialNo { get; set; }

    public bool insertUserLog(LoginModel user)
    {
        bool status = false;
        try
        {
            CCMSEntities db = new CCMSEntities();

            var sUser = new UsersLogs()
            {
                UserID = user.userid,
                UserName = user.username,
                UserRole = user.userrole,
                BranchID = user.branchid,
                BranchName = user.branchname,
                IPAddress = user.IP,
                LoginTime = DateTime.Now,
                isLoggedIn = true
            };

            db.UsersLogs.Add(sUser);
            db.SaveChanges();

            UserSerialNo = sUser.ID.ToString();

            status = true;
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

    public bool hasException { get; set; }

    public bool FetchUser(string userid, string current_sessionId)
    {
        string UserID = "", db_SessionID = "";
        bool status = false;
        try
        {
            CCMSEntities db = new CCMSEntities();

            var resultSet = db.Users.Where(s => s.Login == userid).ToList();

            resultSet.ForEach(
                a =>
                {
                    UserID = a.Login;
                    db_SessionID = a.SessionID;
                });

            if (db_SessionID.Equals(current_sessionId)) status = true;
        }
        catch (Exception ex)
        {
            status = false;

            hasException = true;

            Utility.LogError(
                          new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
                          ex.ToString(),
                          new StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(),
                          new StackTrace(ex, true).GetFrame(0).GetFileName()
                      );
        }

        return status;
    }

    public bool insertUser(string userid, string sessionid, string supervisor_id, string supervisor_email)
    {
        bool status = false;
        try
        {
            CCMSEntities db = new CCMSEntities();

            var query = db.Users.Where(s => s.Login == userid).ToList();

            if (query.Count() < 1)
            {
                var user = new Users()
                {
                    Login = userid,
                    SessionID = sessionid,
                    Supervisor_id = supervisor_id,
                    Supervisor_email=supervisor_email,
                    LastLoginDate = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();
            }
            else
            {
                var record = query.Single();
                record.SessionID = sessionid;
                record.Supervisor_id = supervisor_id;
                record.Supervisor_email = supervisor_email;
                db.SaveChanges();
            }

            status = true;
        }
        catch (DbEntityValidationException ex1)
        {
            foreach (var eve in ex1.EntityValidationErrors)
            {
                foreach (var ve in eve.ValidationErrors)
                {
                    string error = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);

                    Utility.LogError(
                        new StackTrace(ex1, true).GetFrame(0).GetMethod().Name,
                        ex1.ToString(),
                        new StackTrace(ex1, true).GetFrame(0).GetFileLineNumber().ToString(),
                        new StackTrace(ex1, true).GetFrame(0).GetFileName()
                    );
                }
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

    public bool updateUserLog(string userid, string userSerialNo)
    {
        bool status = false;
        try
        {
            CCMSEntities db = new CCMSEntities();

            var query = db.UsersLogs.Where(s => s.UserID == userid && s.ID.ToString() == UserSerialNo);

            foreach (var s in query)
            {
                UsersLogs usersLog = new UsersLogs();
                usersLog.UserID = s.UserID;
                usersLog.UserName = s.UserName;
                usersLog.UserRole = s.UserRole;
                usersLog.BranchID = s.BranchID;
                usersLog.BranchName = s.BranchName;
                usersLog.IPAddress = s.IPAddress;
                usersLog.LoginTime = s.LoginTime;
                usersLog.LogoutTime = DateTime.Now;
                usersLog.isLoggedIn = false;

                db.UsersLogs.Add(usersLog);
                db.SaveChanges();

                status = true;
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
}

public class UserDTO
{
    public string Status { get; set; }
    public int? Group_Id { get; set; }
    public string GroupName { get; set; }
    public DateTime? LastLoginDate { get; set; }

    public string DefaultPage { get; set; }
}

public class GroupResources_View_DTO
{
    public int Id { get; set; }

    [StringLength(50)]
    public string GroupName { get; set; }

    [StringLength(50)]
    public string Status { get; set; }

    [StringLength(50)]
    public string Resource_Id { get; set; }

    [StringLength(50)]
    public string Resource_Name { get; set; }

    [StringLength(50)]
    public string Defaultpage { get; set; }
}