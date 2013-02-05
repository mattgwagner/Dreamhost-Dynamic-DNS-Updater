using NLog;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DHDns.Library
{
    public class UpdateJob : IJob
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IConfig config = new FileConfig();

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Starting UpdateJob...");

            // Get current IP
            var currentIp = GetCurrentIP(this.config);

            Log.Debug("Retrieved current IP: {0}", currentIp);

            // Get the existing record
            var existingIp = GetDNSRecord(this.config);

            Log.Debug("Retrieved existing DNS Record: {0}", existingIp);

            if (currentIp != existingIp)
            {
                Log.Debug("Existing record did not match retrieved IP, updating!");

                RemoveDNSRecord(this.config, existingIp);

                Log.Debug("Removed existing DNS record.");

                AddDNSRecord(this.config, currentIp);

                Log.Debug("Added new DNS record for hostname.");
            }
        }

        public virtual String GetCurrentIP(IConfig config)
        {
            // TODO Make this swappable?

            var request = WebRequest.CreateHttp("http://www.joshlange.net/cgi/get_ip.pl");

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd().Replace("\n", String.Empty);
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

        public virtual String GetDNSRecord(IConfig config)
        {
            // Send the cmd, get back XML records
            var response = SendCmd(config, "dns-list_records");

            var doc = XDocument.Parse(response);

            // TODO Check if 'A' record
            // TODO Check if 'editable'

            var records = from data in doc.Element("dreamhost").Descendants("data")
                          let r = new
                          {
                              Record = data.Element("record").Value,
                              Value = data.Element("value").Value,
                              Editable = data.Element("editable").Value,
                              Type = data.Element("type").Value
                          }
                          where r.Record == config.Hostname
                          select r;

            // We should have one that matches.
            if (records.Any())
            {
                return records.Single().Value;
            }

            // Otherwise, just return an empty string.
            return String.Empty;
        }

        public virtual void RemoveDNSRecord(IConfig config, String existingIp)
        {
            String cmd = String.Format("dns-remove_record&record={0}&value={1}&type=A", config.Hostname, existingIp);

            var deleteResponse = SendCmd(config, cmd);
        }

        public virtual void AddDNSRecord(IConfig config, String newIpAddress)
        {
            String cmd = String.Format("dns-add_record&record={0}&value={1}&type=A", config.Hostname, newIpAddress);

            var addResponse = SendCmd(config, cmd);
        }

        public virtual String SendCmd(IConfig config, String cmd)
        {
            var request = WebRequest.CreateHttp(String.Format("{0}?key={1}&unique_id={2}&format=XML&cmd={3}",
                config.APIUrl,
                config.APIKey,
                Guid.NewGuid(),
                cmd));

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}