using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;

namespace MileageGauge.CSharp.Implementations.ViewModels
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

        public Action<GetDiagnosticDeviceResponse> GetDiagnosticDeviceComplete
        {
            get; set;
        }

        public Action<LoadVehicleDetailsResponse> LoadVehicleDetailsComplete
        {
            get; set;
        }

        public async Task GetDiagnosticDevice()
        {
            await Task.Delay(10000);

            DiagnosticDeviceConnected = true;

            GetDiagnosticDeviceComplete?.Invoke(new GetDiagnosticDeviceResponse { Success = true });
        }

        public async Task LoadVehicleDetails(bool forceRefresh)
        {
            if (!DiagnosticDeviceConnected)
            {
                LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsResponse { Success = false, Message = "Please connect your phone to the ELM327." });
            }

            if (forceRefresh)
            {
                await Task.Delay(5000);
                throw new NotImplementedException("This feature is not ready yet!");
            }

            var vehicleDetails = new VehicleViewModel()
            {
                VIN = "1C3AN69L24X*", //TODO: not currently including whole vin for security
                Make = "CHRYSLER",
                Model = "Crossfire",
                Year = 2004,
                Option = "Man 6-spd, 6 cyl, 3.2 L",
                CityMPG = 15,
                HighwayMPG = 23,
                CombinedMPG = 18

            };

            CurrentVehicle = vehicleDetails;

            LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsResponse { Success = true });
        }

    }
}