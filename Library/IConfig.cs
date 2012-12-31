using System;
using System.Configuration;

namespace DHDns.Library
{
    public interface IConfig
    {
        String APIUrl { get; }

        String APIKey { get; }

        String Username { get; }

        String Hostname { get; }

        int UpdateInterval { get; }
    }

    /// <summary>
    /// Implementation of the configuration that pulls it's data from the app.config file
    /// </summary>
    public class FileConfig : IConfig
    {
        public string APIUrl { get { return "https://api.dreamhost.com"; } }

        public string APIKey { get { return ConfigurationManager.AppSettings["API_Key"]; } }

        public string Username { get { return ConfigurationManager.AppSettings["DH_User"]; } }

        public string Hostname { get { return ConfigurationManager.AppSettings["Hostname"]; } }

        public int UpdateInterval { get { return int.Parse(ConfigurationManager.AppSettings["Update_Interval_Minutes"]); } }
    }

    public class TestConfig : IConfig
    {
        public string APIUrl { get { return "https://api.dreamhost.com"; } }

        public string APIKey { get; set; }

        public string Username { get; set; }

        public string Hostname { get; set; }

        public int UpdateInterval { get; set; }
    }
}
