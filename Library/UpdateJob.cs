using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace DHDns.Library
{
    public class UpdateJob : IJob
    {
        private readonly IConfig config;

        public UpdateJob(IConfig config)
        {
            this.config = config;
        }

        public void Execute(IJobExecutionContext context)
        {
            // Get existing DNS records
            var existing = GetDNSRecords(this.config).SingleOrDefault(record => record.Hostname == this.config.Hostname);

            // If there wasn't a matching record, nothing to update
            // TODO Create record if one doesn't exist?
            if (existing == null) return;

            // Get current IP
            var currentIp = GetCurrentIP(this.config);

            // Compare existing record to current IP
            if (existing.IPAddress != currentIp)
            {
                // Update the DNS record
                UpdateDNSRecord(this.config, currentIp);
            }
        }

        public virtual IEnumerable<DnsRecord> GetDNSRecords(IConfig config)
        {
            // TODO Get the existing records

            var uri = String.Format("{0}?key={1}&unique_id={2}&cmd={3}",
                config.APIUrl,
                config.APIKey,
                config.Username,
                "dns-list_records");

            var request = WebRequest.CreateHttp(uri);
        }

        public virtual String GetCurrentIP(IConfig config)
        {
            // TODO Make this swappable?

            var request = WebRequest.CreateHttp("http://www.joshlange.net/cgi/get_ip.pl");

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public virtual Boolean UpdateDNSRecord(IConfig config, String newIpAddress)
        {
            // TODO Remove the old record

            var uri = String.Format("{0}?key={1}&unique_id={2}&cmd={3}",
                config.APIUrl,
                config.APIKey,
                config.Username,
                "dns-remove_record");

            var request = WebRequest.CreateHttp(uri);

            // TODO Add the new record

            var uri2 = String.Format("{0}?key={1}&unique_id={2}&cmd={3}",
                config.APIUrl,
                config.APIKey,
                config.Username,
                "dns-add_record");

            var request2 = WebRequest.CreateHttp(uri2);


            return false;
        }

        private class DnsRecord
        {
            public String Hostname { get; set; }
            public String IPAddress { get; set; }
        }
    }
}
