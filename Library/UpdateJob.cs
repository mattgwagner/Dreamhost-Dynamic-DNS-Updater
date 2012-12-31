using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

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
            throw new NotImplementedException();
        }

        public virtual String GetCurrentIP(IConfig config)
        {
            throw new NotImplementedException();
        }

        public virtual Boolean UpdateDNSRecord(IConfig config, String newIpAddress)
        {
            throw new NotImplementedException();
        }

        class DnsRecord
        {
            public String Hostname { get; set; }
            public String IPAddress { get; set; }
        }
    }
}
