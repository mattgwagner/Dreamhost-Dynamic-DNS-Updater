using DHDns.Library;
using NLog;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using Topshelf;

namespace DHDns.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<TaskService>();

                x.SetDescription("DreamHost Dynamic DNS Updater.");
                x.SetDisplayName("DreamHost - DynamicDNS Updater");
                x.SetServiceName("DreamHostDynamicDNSUpdater");

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.EnableServiceRecovery(s => s.RestartService(1)); // restart after 1 minute
            });
        }

        private class TaskService : ServiceControl
        {
            private static readonly Logger Log = LogManager.GetCurrentClassLogger();
            private static readonly IConfig Config = new FileConfig();

            private readonly IScheduler scheduler;

            public TaskService()
            {
                this.scheduler = new StdSchedulerFactory().GetScheduler();
                this.scheduler.JobFactory = new NinjectJobFactory(new IOCModule());
            }

            public bool Start(HostControl hostControl)
            {
                Log.Trace("Starting Service...");

                var job = new JobDetailImpl("UpdateJob", typeof(UpdateJob));

                var trigger = TriggerBuilder.Create()
                    .ForJob(job)
                    .WithCalendarIntervalSchedule(x => x.WithIntervalInMinutes(Config.UpdateInterval))
                    .StartNow()
                    .Build();

                this.scheduler.ScheduleJob(trigger);

                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                Log.Trace("Stopping Service...");

                this.scheduler.Standby();

                return true;
            }
        }
    }
}