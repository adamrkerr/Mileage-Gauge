using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MileageGauge.Abstractions;
using MileageGauge.Abstractions.ResponseModels;
using Java.Lang;

namespace MileageGauge.Implementations
{
    public class MainViewModel : IMainViewModel
    {
        public IVehicleViewModel CurrentVehicle
        {
            get; private set;
        }

        public bool DiagnosticDeviceConnected
        {
            get; private set;
        }

        public Task<GetDiagnosticDeviceResponse> GetDiagnosticDevice()
        {
            //TODO: get real device
            Thread.Sleep(5000);

            DiagnosticDeviceConnected = true;

            return Task.FromResult(new GetDiagnosticDeviceResponse { Success = true });
        }

        public Task<LoadVehicleDetailsResponse> LoadVehicleDetails(bool forceRefresh)
        {
            if (!DiagnosticDeviceConnected)
            {
                return Task.FromResult(new LoadVehicleDetailsResponse { Success = false, Message = "Please connect your phone to the ELM327." });
            }

            if (forceRefresh)
            {
                Thread.Sleep(1000);
                throw new NotImplementedException("This feature is not ready yet!");
            }

            var vehicleDetails = new VehicleViewModel()
            {
                VIN = "1C3AN69L24X*", //TODO: not currently including whole vin for security
                Make = "CHRYSLER",
                Model="Crossfire",
                Year=2004,
                Option= "Man 6-spd, 6 cyl, 3.2 L",
                CityMPG=15,
                HighwayMPG=18,
                CombinedMPG=23

            };

            CurrentVehicle = vehicleDetails;

            return Task.FromResult(new LoadVehicleDetailsResponse { Success = true });
        }
    }
}