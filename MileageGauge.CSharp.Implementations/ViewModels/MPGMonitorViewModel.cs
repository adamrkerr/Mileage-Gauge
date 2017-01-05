using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class MPGMonitorViewModel : IMPGMonitorViewModel
    {
        private readonly IDiagnosticDeviceService _diagnosticDeviceService;
        public MPGMonitorViewModel(IDiagnosticDeviceService diagnosticeService)
        {
            _diagnosticDeviceService = diagnosticeService;

            MonitorFlag = false;
        }

        bool MonitorFlag { get; set; }

        Task MonitorTask { get; set; }

        public Action<MPGUpdateResponse> UpdateMPG
        {
            get; set;
        }

        public async Task BeginMonitoringMPG()
        {
            MonitorFlag = true;

            MonitorTask = Task.Factory.StartNew(async () =>
            {
                while (MonitorFlag)
                {
                    await Task.Delay(500);
                    Random rnd = new Random();
                    int mpgInt = rnd.Next(1, 50);
                    int throttleInt = rnd.Next(1, 101);

                    UpdateMPG?.Invoke(new MPGUpdateResponse() { Success = true, CurrentThrottlePercentage = throttleInt, InstantMPG = mpgInt });
                }
            });
        }

        public async Task EndMonitoringMPG()
        {
            MonitorFlag = false;
            MonitorTask?.Wait();
        }
    }
}
