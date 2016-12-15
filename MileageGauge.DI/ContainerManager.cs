using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MileageGauge.Abstractions;
using MileageGauge.Implementations;

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

            return builder.Build();
        }
    }
}
