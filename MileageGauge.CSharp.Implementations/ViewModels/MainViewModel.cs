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
        private readonly IDiagnosticDeviceService _diagnosticService;

        public MainViewModel(IVehicleInformationService vehicleInformationService, IDiagnosticDeviceService diagnosticService)
        {
            _vehicleInformationService = vehicleInformationService;
            _diagnosticService = diagnosticService;
        }

        public IVehicleViewModel CurrentVehicle
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

        public Action<LoadVehicleDetailsModelRequiredResponse> LoadVehicleDetailsModelRequired
        {
            get;set;
        }

        public async Task GetDiagnosticDevice()
        {
            var connected = await _diagnosticService.Connect();

            GetDiagnosticDeviceComplete?.Invoke(new GetDiagnosticDeviceResponse { Success = connected });
        }

        public async Task LoadVehicleDetails(bool forceRefresh)
        {
            if (!_diagnosticService.IsConnected)
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
                Model = vehicleDetails.SelectedModel,
                Year = vehicleDetails.Year
            };

            if (vehicleDetails.SelectedModel == null)
            {
                var models = vehicleDetails.VehicleModels.ToList();

                LoadVehicleDetailsModelRequired?.Invoke(new LoadVehicleDetailsModelRequiredResponse { Success = true, Message = "Please select the vehicle's model.", ModelOptions = models });
                return;
            }

            await ProcessVehicleOptions(vehicleDetails);

        }

        private async Task ProcessVehicleOptions(VehicleInformationResponse vehicleDetails)
        {
            if (vehicleDetails.SelectedVehicleOption == null)
            {
                var options = vehicleDetails.VehicleOptions.Select(s => new VehicleOptionViewModel { Id = s.Id, Text = s.Text }).ToList();

                LoadVehicleDetailsOptionsRequired?.Invoke(new LoadVehicleDetailsOptionRequiredResponse { Success = true, Message = "Please select the vehicle's drivetrain.", VehicleOptions = options });
                return;
            }

            await SetVehicleOptionAndGetMileage(vehicleDetails.SelectedVehicleOption.Id, vehicleDetails.SelectedVehicleOption.Text);

            LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = false });
            return;
        }

        public async Task CompleteVehicleOption(VehicleOptionViewModel selectedOption)
        {
            await SetVehicleOptionAndGetMileage(selectedOption.Id, selectedOption.Text);

            LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = true, DetailsAreFromStorage = false });
        }

        private async Task SetVehicleOptionAndGetMileage(int selectedOptionId, string selectedOptionText)
        {
            CurrentVehicle.Option = selectedOptionText;
            
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

        public async Task CompleteVehicleModel(string selectedModel)
        {
            CurrentVehicle.Model = selectedModel;

            var vehicleDetails = await _vehicleInformationService.GetVehicleInformation(CurrentVehicle.Year, CurrentVehicle.Make, CurrentVehicle.Model);

            await ProcessVehicleOptions(vehicleDetails);
        }
    }
}