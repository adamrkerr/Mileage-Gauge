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

        private decimal GallonsUsed { get; set; }

        private decimal MilesTravelled { get; set; }

        public double PreviousMPH { get; set; }

        public double PreviousGPH { get; set; }

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

            PreviousGPH = 0;

            PreviousMPH = 0;

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

                    GallonsUsed += GetGallonsUsed(gph, PreviousGPH, timeOfCurrentSample, TimeOfLastSample);

                    MilesTravelled += GetMilesTravelled(mph, PreviousMPH, timeOfCurrentSample, TimeOfLastSample);
                    
                    if (GallonsUsed != 0)
                    {
                        AverageMPG = (double)(MilesTravelled / GallonsUsed);
                    }

                    UpdateMPG?.Invoke(new MPGUpdateResponse()
                    {
                        Success = true,
                        CurrentThrottlePercentage = throttleInt,
                        CurrentMPH = mph,
                        InstantMPG = instantMpg,
                        AverageMPG = AverageMPG
                    });

                    PreviousMPH = mph;

                    PreviousGPH = gph;
                    
                    TimeOfLastSample = timeOfCurrentSample;
                }
            });
        }

        private static decimal GetGallonsUsed(double instantGallonsPerHour, double previousGallonsPerHour, DateTime timeOfCurrentSample, DateTime timeOfPreviousSample)
        {
            return GetAmountOverTime(instantGallonsPerHour, previousGallonsPerHour, timeOfCurrentSample, timeOfPreviousSample);
        }

        private static decimal GetMilesTravelled(double instantMilesPerHour, double previousMilesPerHour, DateTime timeOfCurrentSample, DateTime timeOfPreviousSample)
        {
            return GetAmountOverTime(instantMilesPerHour, previousMilesPerHour, timeOfCurrentSample, timeOfPreviousSample);
        }

        private static decimal GetAmountOverTime(double currentReadingPerHour, double previousReadingPerHour, DateTime currentTime, DateTime previousTime)
        {
            if (previousTime == null || previousTime == DateTime.MinValue)
                return 0m;

            var timeSpan = currentTime - previousTime;

            var timeSpanInHours = timeSpan.TotalHours;

            decimal acceleration = (decimal)(currentReadingPerHour - previousReadingPerHour) / (decimal)timeSpanInHours;

            var amount = ((decimal)previousReadingPerHour * (decimal)timeSpanInHours) + (.5m * (acceleration) * ((decimal)Math.Pow(timeSpanInHours, 2)));

            return amount;
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
