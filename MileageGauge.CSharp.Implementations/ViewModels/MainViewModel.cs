using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services;
using System.Collections.Generic;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Linq;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private readonly IVehicleInformationService _vehicleInformationService;
        public MainViewModel(IVehicleInformationService vehicleInformationService)
        {
            _vehicleInformationService = vehicleInformationService;
        }

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

        public Action<LoadVehicleDetailsCompleteResponse> LoadVehicleDetailsComplete
        {
            get; set;
        }

        public Action<LoadVehicleDetailsOptionRequiredResponse> LoadVehicleDetailsOptionsRequired
        {
            get; set;
        }

        public async Task GetDiagnosticDevice()
        {
            await Task.Delay(5000);

            DiagnosticDeviceConnected = true;

            GetDiagnosticDeviceComplete?.Invoke(new GetDiagnosticDeviceResponse { Success = true });
        }

        public async Task LoadVehicleDetails(bool forceRefresh)
        {
            if (!DiagnosticDeviceConnected)
            {
                LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = false, Message = "Please connect your phone to the ELM327." });
            }

            if (!(forceRefresh || CurrentVehicle == null))
            {
                LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = true });
                return;
            }

            var vehicleDetails = await _vehicleInformationService.GetVehicleInformation();

            CurrentVehicle = new VehicleViewModel()
            {
                VIN = vehicleDetails.VIN,
                Make = vehicleDetails.Make,
                Model = vehicleDetails.Model,
                Year = vehicleDetails.Year
            };

            if (vehicleDetails.SelectedVehicleOption != null)
            {                
                await GetVehicleMileage(vehicleDetails.SelectedVehicleOption.Id, vehicleDetails.SelectedVehicleOption.Text);

                LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = false });
                return;
            }

            var options = vehicleDetails.VehicleOptions.Select(s => new VehicleOptionViewModel { Id = s.Id, Text = s.Text }).ToList();

            LoadVehicleDetailsOptionsRequired?.Invoke(new LoadVehicleDetailsOptionRequiredResponse { Success = true, Message = "Please select the vehicle's drivetrain.", VehicleOptions = options });

        }

        public async Task CompleteVehicleDetails(VehicleOptionViewModel selectedOption)
        {
            await GetVehicleMileage(selectedOption.Id, selectedOption.Text);

            LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = false });
        }

        private async Task GetVehicleMileage(int selectedOptionId, string selectedOptionText)
        {
            CurrentVehicle.Option = selectedOptionText;
            //TODO: use option to get MPG
            var response = await _vehicleInformationService.GetVehicleMileageRating(selectedOptionId);

            CurrentVehicle.CityMPG = response.CityMpg;
            CurrentVehicle.CombinedMPG = response.CombinedMpg;
            CurrentVehicle.HighwayMPG = response.HighwayMpg;
        }

        public async Task ContinueWithoutVehicleDetails()
        {
            await Task.Run(() => {
                CurrentVehicle.Option = "Unknown";
                CurrentVehicle.HighwayMPG = 0;
                CurrentVehicle.CombinedMPG = 0;
                CurrentVehicle.CityMPG = 0;
            });

            LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = false });
        }
    }
}