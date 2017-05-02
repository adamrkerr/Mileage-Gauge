using Autofac;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Implementations.ViewModels;
using MileageGauge.CSharp.Implementations.Services;

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
            builder.RegisterType<DiagnosticDeviceService>().As<IDiagnosticDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleInformationService>().As<IVehicleInformationService>();
            builder.RegisterType<RestUtility>().As<IRestUtility>();
            builder.RegisterType<MPGMonitorViewModel>().As<IMPGMonitorViewModel>();

            var container = builder.Build();

            container.BeginLifetimeScope();

            return container;
        }
    }
}
