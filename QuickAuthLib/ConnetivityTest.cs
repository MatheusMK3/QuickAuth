using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QuickAuthLib
{
    public class ConnetivityTest
    {
        public static bool Check(string url)
        {
            // Create HTTP Web Request and Response objects
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            // Retrieve response URL and Status
            string resUrl = res.ResponseUri.ToString();
            int resStatus = (int)res.StatusCode;

            // Validate if page has redirected or returned something other than 2XX response
            bool hasRedirected = (url != resUrl);
            bool hasStatus2XX = ((int)res.StatusCode >= 200 && (int)res.StatusCode < 300);

            // If it's the page we are looking for, return true
            if (!hasRedirected && hasStatus2XX)
                return true;

            // Else return false
            return false;
        }
    }
}
