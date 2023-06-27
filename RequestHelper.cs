using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace PetStoreFrontEnd
{
    class RequestHelper
    {
        // WriteJSON can be used by post/patch functions
        private void WriteJSON(WebRequest req, string jsonString)
        {
            using (var writer = req.GetRequestStream())
            {
                byte[] byteArr = Encoding.ASCII.GetBytes(jsonString);
                writer.Write(byteArr);
                writer.Flush();
            }
        }

        // Handles all post requests
        public string[] PostRequest(string url, JsonObject jsonObject)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Accept = "application/json";
            req.ContentType = "application/json";

            WriteJSON(req, JsonConvert.SerializeObject(jsonObject));
            string statusDescr = "";
            string data = "";
            try
            {
                using var resp = req.GetResponse();
                statusDescr = ((HttpWebResponse)resp).StatusDescription;
                using var reader = new StreamReader(resp.GetResponseStream());
                data = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                data = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }

            // Return status code and JSON response as strings:
            return new string[] { data, statusDescr };
        }

        // Write Patch, Get, and Delete methods here.
        public string[] DeleteRequest(string url, int ID)
        {
            var req = (HttpWebRequest)WebRequest.Create(url + $"/{ID}");
            req.Method = "DELETE";
            req.Accept = "application/json";

            string statusDescr = "";
            string data = "";
            try
            {
                using var resp = req.GetResponse();
                statusDescr = ((HttpWebResponse)resp).StatusDescription;
                using var reader = new StreamReader(resp.GetResponseStream());
                data = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                data = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }

            // Return status code and JSON response as strings:
            return new string[] { data, statusDescr };
        }
    }
}
