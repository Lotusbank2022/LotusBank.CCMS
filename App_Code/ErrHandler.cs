using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SunTrustUSSD.Utilities
{
    public class ErrHandler
    {
        public ErrHandler()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void WriteError(string strMessage, string strRequestID)
        {
            try
            {
                Log.Instance.Debug(string.Format("Date:{0}, RequestID:{1}, LogEntry:{2}",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), strRequestID, strMessage));
            }
            catch (Exception ex)
            {
                EventLog eventLog1 = new EventLog();
                eventLog1.Source = "CCMS";
                eventLog1.WriteEntry("Error in: " + HttpContext.Current.Request.Url.ToString());
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Information);
            }
        }
    }
}