using Ninject.Modules;

namespace DHDns.Library
{
    public class IOCModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfig>().To<FileConfig>();
        }
    }
}