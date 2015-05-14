using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DHDns.Library
{
    public interface IConfig
    {
        String APIUrl { get; }

        String APIKey { get; }

        String Username { get; }

        string HostnameCSV { get; }

        StringCollection Hostnames { get; }

        int UpdateInterval { get; }

        String IPLookupService { get; }
    }

    /// <summary>
    /// Implementation of the configuration that pulls it's data from the app.config file
    /// </summary>
    public class FileConfig : IConfig
    {
        CommaDelimitedStringCollectionConverter CSVConverter = new CommaDelimitedStringCollectionConverter();

        public string APIUrl { get { return "https://api.dreamhost.com"; } }

        public string APIKey { get { return ConfigurationManager.AppSettings["API_Key"]; } }

        public string Username { get { return ConfigurationManager.AppSettings["DH_User"]; } }

        public string HostnameCSV { get { return ConfigurationManager.AppSettings["HostnameCSV"]; } }

        //this piece of condensed code allows for the retrieval of a Comma Separated Value list of hostnames. Note that the output is of type System.Collections.StringCollection.
        public StringCollection Hostnames { get { return (CommaDelimitedStringCollection)(new CommaDelimitedStringCollectionConverter()).ConvertFrom(ConfigurationManager.AppSettings["HostnameCSV"]); } }

        public int UpdateInterval { get { return int.Parse(ConfigurationManager.AppSettings["Update_Interval_Minutes"]); } }

        public string IPLookupService { get { return ConfigurationManager.AppSettings["IP_Lookup_Service"]; } }
    }

    public class TestConfig : IConfig
    {
        public string APIUrl { get { return "https://api.dreamhost.com"; } }

        public string APIKey { get; set; }

        public string Username { get; set; }

        public string HostnameCSV { get; set; }

        public StringCollection Hostnames { get; set; }

        public int UpdateInterval { get; set; }

        public String IPLookupService { get; set; }
    }
}