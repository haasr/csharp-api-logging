using System;
using System.Collections.Generic;
using System.Text;

namespace PetStoreFrontEnd
{
    class JsonObject { }
    // Create Objects to represent each table in monitoring DB.
    class Session : JsonObject
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public long Visited_Timestamp { get; set; }

        public Session(string Country, string Region)
        {
            this.Country = Country;
            this.Region = Region;
        }

        public Session(string Country, string Region, long Visited_Timestamp)
        {
            this.Country = Country;
            this.Region = Region;
            this.Visited_Timestamp = Visited_Timestamp;
        }
    }

    class PageView : JsonObject
    {
        public string Page_Name { get; set; }
        public long Viewed_Timestamp { get; set; }
        public int Session_ID { get; set; }

        public PageView(string Page_Name, long Viewed_Timestamp, int Session_ID)
        {
            this.Page_Name = Page_Name;
            this.Viewed_Timestamp = Viewed_Timestamp;
            this.Session_ID = Session_ID;
        }
    }

    class UserRegistered : JsonObject
    {
        public int User_ID { get; set; }
        public long Reg_Timestamp { get; set; }

        public UserRegistered(int User_ID, long Reg_Timestamp)
        {
            this.User_ID = User_ID;
            this.Reg_Timestamp = Reg_Timestamp;
        }
    }

    class UserOnline : JsonObject
    {
        public int User_ID { get; set; }
        public long Login_Timestamp { get; set; }

        public UserOnline(int User_ID, long Login_Timestamp)
        {
            this.User_ID = User_ID;
            this.Login_Timestamp = Login_Timestamp;
        }
    }
}
