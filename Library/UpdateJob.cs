using NLog;
using Quartz;
using System;
using System.IO;
using System.Net;

namespace DHDns.Library
{
    public class UpdateJob : IJob
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IConfig config;

        public UpdateJob(IConfig config)
        {
            this.config = config;
        }

        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Starting UpdateJob...");

            // Get current IP
            var currentIp = GetCurrentIP(this.config);

            Log.Debug("Retreived current IP: {0}", currentIp);

            // Get the existing record
            var existingIp = GetDNSRecord(this.config);

            Log.Debug("Retrieved existing DNS Record: {0}", existingIp);

            if (currentIp != existingIp)
            {
                Log.Debug("Existing record did not match retrieved IP, updating!");

                RemoveDNSRecord(this.config);

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
                return reader.ReadToEnd();
            }
        }

        public virtual String GetDNSRecord(IConfig config)
        {
            var response = SendCmd(config, "dns-list_records");

            using (var reader = new StringReader(response))
            {
                String line = String.Empty;

                while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    Log.Info("Record Line: {0}", line);

                    // TODO Filter on hostname?

                    return line;
                }
            }

            return String.Empty;
        }

        public virtual Boolean RemoveDNSRecord(IConfig config)
        {
            // TODO Remove the old record

            var deleteResponse = SendCmd(config, "dns-delete_record", config.Hostname);

            return false;
        }

        public virtual Boolean AddDNSRecord(IConfig config, String newIpAddress)
        {
            // TODO Add the new record

            var addResponse = SendCmd(config, "dns-add_record", newIpAddress);

            return false;
        }

        public virtual String SendCmd(IConfig config, String cmd, String data = "")
        {
            var request = WebRequest.CreateHttp(String.Format("{0}?key={1}&unique_id={2}&cmd={3}",
                config.APIUrl,
                config.APIKey,
                config.Username,
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