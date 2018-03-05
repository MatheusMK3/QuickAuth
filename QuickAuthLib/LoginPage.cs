using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QuickAuthLib
{
    public class LoginPage
    {
        public Dictionary<string, string> Fields { get; protected set; }
        public string UsernameField { get; protected set; }
        public string PasswordField { get; protected set; }
        public string SubmitURL { get; protected set; }

        protected LoginPage(string htmlCode)
        {
            // Initialize Fields
            this.Fields = new Dictionary<string, string>();

            // Initialize HTML Document
            HtmlDocument dom = new HtmlDocument();
            dom.LoadHtml(htmlCode);

            // Find all <input> tags
            HtmlNodeCollection inputs = dom.DocumentNode.SelectNodes("//td/input");

            // Find our username and password fields
            HtmlNode previousNode = null;
            foreach (HtmlNode input in inputs) {
                // Add fields with default values to our "Fields" array, just in case there's some sort of validation like CSRF
                if (input.Attributes.Contains("value"))
                    this.Fields.Add(input.Attributes["name"].Value, input.Attributes["value"].Value);
                
                // Check if current field is of "password" type
                if (input.Attributes["type"].Value.ToLower() == "password")
                {
                    // Save password field
                    this.PasswordField = input.Attributes["name"].Value;

                    // Save previous field as "username"
                    this.UsernameField = previousNode.Attributes["name"].Value;
                }

                previousNode = input;
            }

            // Form element
            HtmlNode form = LoginPage.FindForm(previousNode);

            // Submit URL
            this.SubmitURL = form.Attributes["action"].Value;

            this.PasswordField = this.PasswordField;
        }
        private static HtmlNode FindForm(HtmlNode field)
        {
            // Is it a form
            if (field.Name.ToLower() == "form")
                return field;

            // Has parent
            if (field.ParentNode == null)
                return null;

            // Test parent
            return (LoginPage.FindForm(field.ParentNode));
        }
        public static LoginPage Load(string url) {
            // Look up login page from URL
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            // Create streams for reading
            Stream resStream = res.GetResponseStream();
            StreamReader resStreamRead = new StreamReader(resStream);

            // Read the response HTML code
            string htmlCode = resStreamRead.ReadToEnd();

            // Close streams
            resStreamRead.Close();
            resStream.Close();

            // Return new LoginPage from our HTML
            return LoginPage.LoadFromHtml(htmlCode);
        }
        public static LoginPage LoadFromHtml(string htmlCode)
        {
            // Initialize object and return from HTML (maybe move into construct)
            return new LoginPage(htmlCode);
        }
        public void Login(string username, string password)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection postData = new NameValueCollection();

                // Feed our Fields to postData
                foreach(var field in this.Fields)
                {
                    postData[field.Key] = field.Value;
                }

                // Feed username and password
                postData[this.UsernameField] = username;
                postData[this.PasswordField] = password;
                
                // POST to server
                byte[] response = client.UploadValues(this.SubmitURL, postData);
            }
        }
    }
}
