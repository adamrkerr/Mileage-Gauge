using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class AddVehicleViewModel : IAddVehicleViewModel
    {
        private readonly IVehicleInformationService _vehicleInformationService;
        private readonly IDiagnosticDeviceService _diagnosticService;
        private readonly IVehicleHistoryService _vehicleHistoryService;

        public AddVehicleViewModel(IVehicleInformationService vehicleInformationService, IDiagnosticDeviceService diagnosticService, IVehicleHistoryService vehicleHistoryService)
        {
            _vehicleInformationService = vehicleInformationService;
            _diagnosticService = diagnosticService;
            _vehicleHistoryService = vehicleHistoryService;
        }

        public VehicleModel CurrentVehicle { get; set; }

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
            get; set;
        }

        public async Task LoadVehicleDetailsFromDevice()
        {
            if (!_diagnosticService.IsConnected)
            {
                LoadVehicleDetailsComplete?.Invoke(new LoadVehicleDetailsCompleteResponse { Success = false, Message = "Please connect your phone to the ELM327." });
            }
            
            var vin = await _diagnosticService.GetVin();

            await LoadVehicleDetailsFromVin(vin);
        }

        public async Task LoadVehicleDetailsFromVin(string vin)
        {
            var vehicleDetails = await _vehicleInformationService.GetVehicleInformation(vin);

            CurrentVehicle = new VehicleModel()
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

        public async Task CompleteVehicleModel(string selectedModel)
        {
            CurrentVehicle.Model = selectedModel;

            var vehicleDetails = await _vehicleInformationService.GetVehicleInformation(CurrentVehicle.Year, CurrentVehicle.Make, CurrentVehicle.Model);

            await ProcessVehicleOptions(vehicleDetails);
        }

        public async Task<AddVehicleToCollectionResponse> AddVehicleToCollection()
        {
            try
            {
                await _vehicleHistoryService.AddOrUpdateVehicle(CurrentVehicle);

                return new AddVehicleToCollectionResponse { Success = true };
            }
            catch(Exception ex)
            {
                return new AddVehicleToCollectionResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
