using EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class clsEmailNotification
    {
        public void notifyStakeholders(CustomerComplaints complaint, string supervisorEmail, string complaintCategory, string complaintSubCategory, string internalSLADuration, string stakeholders)
        {
            try
            {
            string subject = "CCMS: NEW REQUEST",
                from = complaint.Created_By + "@suntrustng.com",
                to = string.IsNullOrEmpty(stakeholders) ? supervisorEmail : stakeholders,
                message = "",
                Bcc = "",
                Cc = supervisorEmail + ";"+ complaint.Created_By + "@suntrustng.com;" + ConfigurationManager.AppSettings["cc_email"].ToString();

            message = "<div style=width:80%;";
                message = message + "<font face=\"Times New Roman\" size=\"12\"><br>";
                message = message + "Dear All,";
                message = message + "<br>";
                message = message + "<br>";
                message = message + "Please be informed that the following customer complaint has been logged awaiting your attention.";
                message = message + "<br>";
                message = message + "<br>";
                message = message + "<table cellpadding=2 cellspacing=0 border=1 style=width:60%;background=cid:imageId3; border-left-width:thin; border-left-color:#003366; border-right-width:thin; border-right-color:#003366; border-right-style: solid; border-left-style: solid;";
                message = message + "<tr>";
                message = message + "<td class=\"text - left\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Tracking Reference:</b> </td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Tracking_Reference_Number + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Name of Complainant:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Name_Of_Complainant + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Account Number:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Account_Number + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Category:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaintCategory + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Sub-Category:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaintSubCategory + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Subject of Complaint:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Subject_Of_Complaint + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Description:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Complaint_Description + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount In Dispute:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" +  complaint.Amount_In_Dispute + "</td>";
                message = message + "</tr>";           
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount Refunded Petitioner:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" +  complaint.Amount_Refunded_Petitioner + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount Recovered by Bank:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" +  complaint.Amount_Recovered_by_Bank + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Status of Complaint:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Status_Of_Complaint + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Action Taken:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Action_Taken + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Remarks:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Remarks_by_Bank + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was received:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Date_Received + "</td>";
                message = message + "</tr>";

                if (complaint.Status_Of_Complaint == "CLOSED" || complaint.Status_Of_Complaint == "RESOLVED")
                {
                    message = message + "<tr>";
                    message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was closed/resolved:</b></td>";
                    message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Date_Closed + "</td>";
                    message = message + "</tr>";
                }
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was logged:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" +  complaint.Date_Created + "</td>";
                message = message + "</tr>";
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Logged by:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Created_By + "</td>";
                message = message + "</tr>";            
                message = message + "</table>";
                message = message + "<br>";
                message = message + "Please note that you have to resolve and communicate back to the branch for closure within the SLA period of " + internalSLADuration + " day(s)";
                message = message + "<br>";
                message = message + "<br>";
                message = message + "Thank you.";
                message = message + "<br>";
                message = message + "</font></div>";


                var q = SunTrustProxy.SendEmail(subject, from, to, message, Bcc, Cc, true);

            }
            catch (Exception ex)
            {
                Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
            }
        }


        public void notifyOnModification(CustomerComplaints complaint, string supervisorEmail, string complaintCategory, string complaintSubCategory)
    {
        try
        {
            string subject = "CCMS: COMPLAINT MODIFICATION",
                from = complaint.Created_By + "@suntrustng.com",
                to = supervisorEmail,
                message = "",
                Bcc = "",
                Cc = supervisorEmail + ";" + complaint.Created_By + "@suntrustng.com";

            message = "<div style=width:80%;";
            message = message + "<font face=\"Times New Roman\" size=\"12\"><br>";
            message = message + "Dear All,";
            message = message + "<br>";
            message = message + "<br>";
            message = message + "Please be informed that the following customer complaint has been modified.";
            message = message + "<br>";
            message = message + "<br>";
            message = message + "<table cellpadding=2 cellspacing=0 border=1 style=width:60%;background=cid:imageId3; border-left-width:thin; border-left-color:#003366; border-right-width:thin; border-right-color:#003366; border-right-style: solid; border-left-style: solid;";
            message = message + "<tr>";
            message = message + "<td class=\"text - left\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Tracking Reference:</b> </td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Tracking_Reference_Number + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Name of Complainant:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Name_Of_Complainant + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Account Number:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Account_Number + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Category:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaintCategory + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Sub-Category:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaintSubCategory + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Subject of Complaint:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Subject_Of_Complaint + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Complaint Description:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Complaint_Description + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount In Dispute:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Amount_In_Dispute + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount Refunded Petitioner:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Amount_Refunded_Petitioner + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Amount Recovered by Bank:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Amount_Recovered_by_Bank + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Status of Complaint:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Status_Of_Complaint + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Action Taken:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Action_Taken + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Remarks:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Remarks_by_Bank + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was received:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Date_Received + "</td>";
            message = message + "</tr>";

            if (complaint.Status_Of_Complaint == "CLOSED" || complaint.Status_Of_Complaint == "RESOLVED")
            {
                message = message + "<tr>";
                message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was closed/resolved:</b></td>";
                message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Date_Closed + "</td>";
                message = message + "</tr>";
            }
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Complaint was logged:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + string.Format("dd-MM-yyyy hh:mm:ss", complaint.Date_Created) + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Logged by:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Created_By + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Date Modified:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Date_Modifiied + "</td>";
            message = message + "</tr>";
            message = message + "<tr>";
            message = message + "<td class=\"auto - style2\" style=\"border - style: inset; border - width: thin; width: 190px; \"><b>Modified by:</b></td>";
            message = message + "<td style=\"border - style: inset; border - width: thin\">" + complaint.Modified_By + "</td>";
            message = message + "</tr>";
            message = message + "</table>";
            message = message + "<br>";
            message = message + "<br>";
            message = message + "<br>";
            message = message + "Thank you.";
            message = message + "<br>";
            message = message + "</font></div>";


            var q = SunTrustProxy.SendEmail(subject, from, to, message, Bcc, Cc, true);

        }
        catch (Exception ex)
        {
            Utility.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + " : " + ex.StackTrace);
        }
    }
}

