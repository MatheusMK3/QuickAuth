﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QuickAuthLib
{
    public class LoginPage
    {
        public string UsernameField { get; private set; }
        public string PasswordField { get; private set; }
        public string SubmitURL { get; private set; }

        protected LoginPage(string htmlCode)
        {
            HtmlDocument dom = new HtmlDocument();
            dom.LoadHtml(htmlCode);

            HtmlNodeCollection inputs = dom.DocumentNode.SelectNodes("//td/input");

            // Find our username and password fields
            HtmlNode previousNode = null;
            foreach (HtmlNode input in inputs) {
                if (input.Attributes["type"].Value.ToLower() == "password")
                {
                    this.PasswordField = input.Attributes["name"].Value;
                    this.UsernameField = previousNode.Attributes["name"].Value;
                    break;
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
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(this.SubmitURL);
        }
    }
}
