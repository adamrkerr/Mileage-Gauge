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

        double PointsSampled { get; set; }

        double AverageMPG { get; set; }

        public Action<MPGUpdateResponse> UpdateMPG
        {
            get; set;
        }

        public async Task BeginMonitoringMPG()
        {
            MonitorFlag = true;

            PointsSampled = 0;

            AverageMPG = 0;

            MonitorTask = Task.Factory.StartNew(async () =>
            {
                while (MonitorFlag)
                {
                    var rnd = new Random();
                    var throttleInt = await _diagnosticDeviceService.GetThrottlePercentage();
                    var mph = await _diagnosticDeviceService.GetMPH();
                    var gph = await _diagnosticDeviceService.GetGPH();

                    var instantMpg = mph / gph;

                    PointsSampled++;

                    AverageMPG = (AverageMPG * ((PointsSampled - 1) / PointsSampled)) + (instantMpg / PointsSampled);

                    UpdateMPG?.Invoke(new MPGUpdateResponse()
                    {
                        Success = true,
                        CurrentThrottlePercentage = throttleInt,
                        CurrentMPH = mph,
                        InstantMPG = instantMpg,
                        AverageMPG = AverageMPG
                    });
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
