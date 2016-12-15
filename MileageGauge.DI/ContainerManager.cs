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
        public static IContainer Container { get; set; }

        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType<MainViewModel>().As<IMainViewModel>();

            Container = builder.Build();
        }
    }
}
