using Autofac;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Implementations.ViewModels;

namespace MileageGauge.DI
{
    public static class ContainerManager
    {
        private static IContainer _container;
        public static IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = Initialize();
                }

                return _container;
            }
        }

        private static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainViewModel>().As<IMainViewModel>().InstancePerLifetimeScope();

            var container = builder.Build();

            container.BeginLifetimeScope();

            return container;
        }
    }
}
