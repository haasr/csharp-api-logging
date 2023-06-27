using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PetStoreFrontEnd.Utilities
{
    public class APILogger
    {
        private static string _MONITORING_API_URL_PREFIX = "https://teamducks.herokuapp.com/api/monitoring";
        private static string _IPINFO_TOKEN = "XXXXXXXXXXX"; // Plz don't steal Ryan's personal IPInfo token -- get your own, fiends!

        private static RequestHelper _requestHelper = new RequestHelper();
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;

        //private struct LogResult { int status { get; set; } }
        private static async ValueTask<int> GetLogReturnInt(String[] resp)
        {
            if (resp[1].Equals("Created")) // Successful POST; return ID from API.
            {
                return Int32.Parse(Regex.Replace(resp[0], "[^0-9]", ""));
            }

            return 0; // Otherwise, POST failed; return 0 to indicate failure.
        }

        public static async ValueTask<int> LogUserRegistered(int appUserID)
        {
            UserRegistered u = new UserRegistered(appUserID, GetCurrentUnixTimestamp());

            var resp = _requestHelper.PostRequest(_MONITORING_API_URL_PREFIX + "/users/registered", u);
            _httpContext.Session.SetInt32("User_ID", appUserID);

            int status = await GetLogReturnInt(resp);
            return status;
        }

        public static async ValueTask<int> LogUserOnline(int appUserID)
        {
            UserOnline u = new UserOnline(appUserID, GetCurrentUnixTimestamp());

            var resp = _requestHelper.PostRequest(_MONITORING_API_URL_PREFIX + "/users/online", u);
            _httpContext.Session.SetInt32("User_ID", appUserID);

            int status = await GetLogReturnInt(resp);
            return status;
        }

        public static async ValueTask<int> DeleteUserOnline()
        {
            try
            {
                var appUserID = (int)_httpContext.Session.GetInt32("User_ID");
                var resp = _requestHelper.DeleteRequest(_MONITORING_API_URL_PREFIX + "/users/online", appUserID);

                int status = await GetLogReturnInt(resp);
                return status;
            }
            catch { return 0; }
        }

        public static async ValueTask<int> LogSession()
        {
            if (_httpContext.Session.GetInt32("Session_ID") == null)
            {
                string ipAddr = _httpContext.Connection.RemoteIpAddress.ToString();
                Session session = GetLocationByIP(ipAddr);
                session.Visited_Timestamp = GetCurrentUnixTimestamp();

                var resp = _requestHelper.PostRequest(_MONITORING_API_URL_PREFIX + "/usage/visits", session);

                int sessionID = await GetLogReturnInt(resp); // Successful: returns ID. Unsuccessful: returns 0.
                _httpContext.Session.SetInt32("Session_ID", sessionID); // Set the session ID so each page can be logged.

                int status = await GetLogReturnInt(resp);
                return status;
            }
            return 0;
        }

        public static async ValueTask<int> LogPageView(string pageName)
        {
            await LogSession();

            PageView p = new PageView(pageName, GetCurrentUnixTimestamp(),
                                        (int)_httpContext.Session.GetInt32("Session_ID"));

            var resp = _requestHelper.PostRequest(_MONITORING_API_URL_PREFIX + "/usage/views", p);

            int status = await GetLogReturnInt(resp);
            return status;
        }

        private static long GetCurrentUnixTimestamp()
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            return (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
        }

        private static Session GetLocationByIP(string IP)
        {
            string url = $"https://ipinfo.io/{IP}?token={_IPINFO_TOKEN}";
            var request = WebRequest.Create(url);

            using (WebResponse wrs = request.GetResponse())
            {
                using (Stream stream = wrs.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        var obj = JObject.Parse(json);
                        string region = (string)obj["region"];
                        string country = (string)obj["country"];

                        return new Session(country, region);
                    }
                }
            }
        }
    }
}
