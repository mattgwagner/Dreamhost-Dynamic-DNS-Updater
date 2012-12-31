using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
