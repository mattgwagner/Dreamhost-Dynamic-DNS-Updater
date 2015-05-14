using NLog;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;

namespace DHDns.Library
{
    public class UpdateJob : IJob
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IConfig config = new FileConfig();

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Starting UpdateJob...");

            var currentIp = GetCurrentIP(this.config);
            if (currentIp == null)
            {
                Log.Info("Cancelling UpdateJob...");
                return;
            }

            Log.Info("Retrieved current IP: {0}", currentIp);

            foreach (KeyValuePair<string, string> d in GetDNSRecords(this.config))
            {
                if (currentIp != d.Value)
                {
                    Log.Info("Existing record {0} did not match retrieved IP, updating!", d.Key);

                    RemoveDNSRecord(d.Key, d.Value);

                    Log.Info("Removed existing DNS record.");

                    AddDNSRecord(d.Key, currentIp);

                    Log.Info("Added new DNS record for {0}.", d.Key);
                }
                else
                {
                    Log.Info("DNS record matches, finshing.");
                }
            }

            Log.Info("Finished UpdateJob");
        }

        public virtual String GetCurrentIP(IConfig config)
        {
            var request = WebRequest.CreateHttp(config.IPLookupService);
            request.Timeout = 15000;
            try
            {
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd().Replace("\n", String.Empty);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("Failed to retrieve IP", ex);
                return null;
            }
        }

        /*
         * <?xml version="1.0"?>
         * <dreamhost>
             *  <data>
             *      <account_id>687777</account_id>
             *      <comment/>
             *      <editable>1</editable>
             *      <record>home.mattgwagner.com</record>
             *      <type>A</type>
             *      <value>70.127.40.169</value>
             *      <zone>mattgwagner.com</zone>
             *  </data>
         *  </dreamhost>
         */
        //TKey is hostname, TValue is existing IP
        public virtual IEnumerable<KeyValuePair<string, string>> GetDNSRecords(IConfig config)
        {
            // Send the cmd, get back XML records
            var response = SendCmd(config, "dns-list_records");

            XDocument doc;

            try
            {
                doc = XDocument.Parse(response);
            }
            catch (Exception ex)
            {
                Log.ErrorException("failed to parse DNS records from DreamHost", ex);
                return new List<KeyValuePair<string, string>>(); //return an empty list to prevent cascading exceptions
            };

            //Take each record, check if it matches an entry in the config.Hostnames StringCollection, then compile a list from the records and values that were selected.
            var records = from data in doc.Element("dreamhost").Descendants("data")
                          let r = new
                          {
                              Record = data.Element("record").Value,
                              Value = data.Element("value").Value,
                              Editable = data.Element("editable").Value,
                              Type = data.Element("type").Value
                          }
                          where config.Hostnames.Contains(r.Record) && r.Editable == "1" && r.Type.Equals("A") //make sure that r is one of the records we want(I.E., listed in appconfig and editable)
                          select new KeyValuePair<string, string>(r.Record, r.Value);
            Log.Debug("Retrieved existing DNS Records. Matching Records Count: {0}", records.Count());

            return records; //records may be empty upon return
        }

        public virtual void RemoveDNSRecord(String hostname, String existingIp)
        {
            String cmd = String.Format("dns-remove_record&record={0}&value={1}&type=A", hostname, existingIp);

            var deleteResponse = SendCmd(config, cmd);

            Log.Debug("Response after issuing DNS Record delete command: " + deleteResponse); //these responses may be helpful debugging info.

        }

        public virtual void AddDNSRecord(String hostname, String newIpAddress)
        {
            String cmd = String.Format("dns-add_record&record={0}&value={1}&type=A", hostname, newIpAddress);

            var addResponse = SendCmd(config, cmd);

            Log.Debug("Response after issuing add DNS Record command: " + addResponse); //these responses may be helpful debugging info.
        }

        public virtual String SendCmd(IConfig config, String cmd)
        {
            try
            {
                var request = WebRequest.CreateHttp(String.Format("{0}?key={1}&unique_id={2}&format=XML&cmd={3}",
                    config.APIUrl,
                    config.APIKey,
                    Guid.NewGuid(),
                    cmd));

                var response = request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception Ex)
            {
                Log.ErrorException("There was a problem communicating with DreamHost. Check your APIKey", Ex);
                return String.Empty;
            }
        }
    }
}