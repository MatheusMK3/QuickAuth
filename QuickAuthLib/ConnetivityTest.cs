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
            ConnectivityTestResults empty;
            return Check(url, out empty);
        }
        public static bool Check(string url, out ConnectivityTestResults results)
        {
            results = new ConnectivityTestResults();
            results.RequestURL = url;

            // Create HTTP Web Request and Response objects
            HttpWebRequest req;
            HttpWebResponse res;

            try
            {
                // Populate HTTP Web Request and Response objects, execute
                req = (HttpWebRequest)WebRequest.Create(url);
                res = (HttpWebResponse)req.GetResponse();

                results.HasInternet = true;
            }
            catch
            {
                results.HasPassedTest = false;
                results.HasInternet = false;
                return false;
            }

            // Populate Results Object
            results.ResponseURL = res.ResponseUri.ToString();
            results.StatusCode = (int)res.StatusCode;

            // Validate if page has redirected or returned something other than 2XX response
            results.HasRedirected = (results.RequestURL != results.ResponseURL);
            results.HasStatus2XX = (results.StatusCode >= 200 && results.StatusCode < 300);

            // If it's the page we are looking for, set as passed
            if (!results.HasRedirected && results.HasStatus2XX)
                results.HasPassedTest = true;
            else
                results.HasPassedTest = false;

            // Return passed state
            return results.HasPassedTest;
        }
    }
    public struct ConnectivityTestResults
    {
        public bool HasInternet;
        public bool HasRedirected;
        public bool HasStatus2XX;
        public bool HasPassedTest;
        public string RequestURL;
        public string ResponseURL;
        public int StatusCode;
    }
}
