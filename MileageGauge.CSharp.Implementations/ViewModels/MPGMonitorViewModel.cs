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

        //double PointsSampled { get; set; }

        double AverageMPG { get; set; }

        private DateTime TimeOfLastSample { get; set; }

        private double GallonsUsed { get; set; }

        private double MilesTravelled { get; set; }

        public Action<MPGUpdateResponse> UpdateMPG
        {
            get; set;
        }

        public async Task BeginMonitoringMPG()
        {
            MonitorFlag = true;

            //PointsSampled = 0;

            AverageMPG = 0;

            MilesTravelled = 0;

            GallonsUsed = 0;

            TimeOfLastSample = DateTime.MinValue;

            MonitorTask = Task.Factory.StartNew(async () =>
            {
                while (MonitorFlag)
                {
                    var rnd = new Random();
                    var throttleInt = await _diagnosticDeviceService.GetThrottlePercentage();
                    var mph = await _diagnosticDeviceService.GetMPH();
                    var gph = await _diagnosticDeviceService.GetGPH();

                    var timeOfCurrentSample = DateTime.Now;
                    
                    var instantMpg = mph / gph;

                    GallonsUsed += GetGallonsUsed(gph, timeOfCurrentSample, TimeOfLastSample);

                    MilesTravelled += GetMilesTravelled(mph, timeOfCurrentSample, TimeOfLastSample);
                    
                    if (GallonsUsed != 0)
                    {
                        AverageMPG = MilesTravelled / GallonsUsed;
                    }

                    UpdateMPG?.Invoke(new MPGUpdateResponse()
                    {
                        Success = true,
                        CurrentThrottlePercentage = throttleInt,
                        CurrentMPH = mph,
                        InstantMPG = instantMpg,
                        AverageMPG = AverageMPG
                    });
                    
                    TimeOfLastSample = timeOfCurrentSample;
                }
            });
        }

        private static double GetGallonsUsed(double instantGallonsPerHour, DateTime timeOfCurrentSample, DateTime timeOfPreviousSample)
        {
            if (timeOfPreviousSample == null || timeOfPreviousSample == DateTime.MinValue)
                return 0;

            var timeSpan = timeOfCurrentSample - timeOfPreviousSample;

            var timeSpanInHours = timeSpan.TotalHours;

            return timeSpan.TotalHours * instantGallonsPerHour;
        }

        private static double GetMilesTravelled(double instantMilesPerHour, DateTime timeOfCurrentSample, DateTime timeOfPreviousSample)
        {
            if (timeOfPreviousSample == null || timeOfPreviousSample == DateTime.MinValue)
                return 0;

            var timeSpan = timeOfCurrentSample - timeOfPreviousSample;

            var timeSpanInHours = timeSpan.TotalHours;

            return timeSpan.TotalHours * instantMilesPerHour;
        }

        public async Task EndMonitoringMPG()
        {
            MonitorFlag = false;
            MonitorTask?.Wait();
        }

        public void Dispose()
        {
            Task.WaitAll(EndMonitoringMPG());
        }
    }
}
