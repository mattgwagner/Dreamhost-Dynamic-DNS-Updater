using Ninject;
using Ninject.Modules;
using Quartz;
using Quartz.Spi;

namespace DHDns.Library
{
    public class NinjectJobFactory : IJobFactory
    {
        private readonly IKernel kernel;

        public NinjectJobFactory(params NinjectModule[] modules)
        {
            this.kernel = new StandardKernel(modules);
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return this.kernel.Get(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            // Nothing to do here?
        }
    }
}