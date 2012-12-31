using NLog;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace DHDns
{
    class Program
    {
        static void Main(string[] args)
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

        class TaskService : ServiceControl
        {
            private static readonly Logger Log = LogManager.GetCurrentClassLogger();

            private readonly IScheduler scheduler;

            public TaskService()
            {
                this.scheduler = new StdSchedulerFactory().GetScheduler();
                this.scheduler.JobFactory = new NinjectJobFactory(new IOCModule());
            }

            public bool Start(HostControl hostControl)
            {
                Log.Trace("Starting Service...");

                // TODO Configure Job

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
