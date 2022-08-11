using EF;
using Newtonsoft.Json;
using nfp.ssm.core;
using RestSharp;
using SunTrustUSSD.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
//using System.Net;
//using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

public static class Utility
{

    /// <summary>
    /// This class is used to serialize a Json object to string
    /// </summary>
    /// <seealso cref="System.Net.Http.StringContent" />
    public class JsonContent : StringContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContent"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public JsonContent(object content) : base(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
        {
            var t = JsonConvert.SerializeObject(content);
        }
    }

    public static object ExecuteHttpRequest(object request, string endpoint)
    {
        //dynamic addacctresp = new JObject();
        //string MiddlewareBaseUrl = SunTrustProxy.GetAppConfigValue("sunware");
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {

            var httpClient = new HttpClient();
            response = httpClient.PostAsync(endpoint, new JsonContent(request)).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<object>(responseBody);
                return res;
            }
            else
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<object>(responseBody);
                return res;
            }
        }
        catch (Exception ex)
        {
            //return response;
            ErrHandler.WriteError("ExecuteHttpRequest Error occurred  - " + ex.Message + "||" + ex.InnerException, "");
            return null;
        }
    }


    public static void sendNotification(CustomerComplaints complaint, string supervisorEmail, string complaintCategory, string complaintSubCategory, string categoryCode)
    {
        try
        {
            clsEmailNotification notification = new clsEmailNotification();
            Category cat = fillCategory(categoryCode);

            if (cat != null)
            {
                notification.notifyStakeholders(complaint, supervisorEmail, complaintCategory, complaintSubCategory,
                                    cat.InternalSLADuration, cat.Notify);
            }
        }
        catch(Exception ex) {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }

    public static void notifyOnModification(CustomerComplaints complaint, string supervisorEmail, string complaintCategory, string complaintSubCategory)
    {
        try
        {
            clsEmailNotification notification = new clsEmailNotification();
            notification.notifyOnModification(complaint, supervisorEmail, complaintCategory, complaintSubCategory);

        }
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }

    public static Category fillCategory(string categoryCode)
    {
        Category cat = new Category();
        try
        {
            using (var dbContext = new EF.CCMSEntities())
            {
                var query = (from d in dbContext.ComplaintsCategory
                             join c in dbContext.ComplaintsCategoryMatrix
                             on d.CategoryId equals c.CategoryId
                             where c.CBNCode == categoryCode
                             select new
                             {
                                 d.CategoryName,
                                 d.CategoryId,
                                 c.SubCategory,
                                 c.SLA,
                                 c.InternalSLA,
                                 d.Notify
                             }).ToList();

                query.ForEach(
                    a =>
                    {
                        cat.CategoryName = a.CategoryName;
                        cat.SubCategory = a.SubCategory;
                        cat.CategoryId = a.CategoryId.ToString();
                        cat.SLADuration = a.SLA;
                        cat.Notify = a.Notify;
                        cat.InternalSLADuration = a.InternalSLA;
                    });

            }
        }
        catch (Exception ex) {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
        return cat;
    }

    public static T DeserializeTo<T>(string xml)
    {
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using (TextReader reader = new StringReader(xml))
        {
            T result = (T)serializer.Deserialize(reader);
            return result;
        }
    }
      
    public static void LogError(String method, String message, String linenumber, String otherinfo)
    {
        try
        {
            using (var context = new  CCMSEntities())
            {
                var newentry = new EF.ErrorLog { Method = method, Message = message, LineNumber = linenumber, OtherInfo = otherinfo };
                context.ErrorLog.Add(newentry);
                context.SaveChanges();
            }
        }
        catch { }
    }
    
    public static class UserStatus
    {
        public static string ACTIVE = "ACTIVE";
        public static string DISABLED = "DISABLED";
    }

     

    #region configuration related
 

    public static string GetAppConfigValue(string key)
    {
        return System.Configuration.ConfigurationSettings.AppSettings.GetValues(key)[0].ToString();
    }
  
    public static string channelcode()
    {
        return ConfigurationManager.AppSettings.GetValues("channelcode")[0].ToString();
    }

    public static string SunwareUrl()
    {
        return ConfigurationManager.AppSettings.GetValues("SunwareUrl")[0].ToString();
    }

    public static string StbService()
    {
        return ConfigurationManager.AppSettings.GetValues("stbservice")[0].ToString();
    }

    #endregion


    public  class SaveResponse
    {
      public string code = "";
      public string message = "";

        public SaveResponse(string cd, string msg)
        {
            code = cd;
            message = msg;
        }
    }


    public class ajaxresponse
    {
        public string responsecode { get; set; }
        public string responsemessage { get; set; }
    }

    

}
public class CurrentUser
{
    public bool isreliever = false;
    public string userid;
    public string staffid;
    public string photo;
    public string groupid;
    public string groupname;
    public string lastlogindate;
    public string branch;
    public string email;
    public string usertype;

    public string relievedstaff_userid;
    public string relievedstaff_staffid;
    public string relievedstaff_email;
}

public class Category
{
    public string CategoryName { get; set; }
    public string CategoryId { get; set; }
    public string SubCategory { get; set; }
    public string SLADuration { get; set; }
    public string InternalSLADuration { get; set; }
    public string Notify { get; set; }
}





