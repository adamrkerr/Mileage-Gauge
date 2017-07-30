using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Implementations.ViewModels;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class VehicleInformationService : IVehicleInformationService
    {
        //TODO: move this somewhere else?
        private const string VinAPI = "https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVin/{0}*?format=json";
        private const string OptionsAPI = "https://fueleconomy.gov/ws/rest/vehicle/menu/options?year={0}&make={1}&model={2}";
        private const string ModelsAPI = "https://fueleconomy.gov/ws/rest/vehicle/menu/model?year={0}&make={1}";
        private const string MileageAPI = "https://fueleconomy.gov/ws/rest/vehicle/{0}";

        private readonly IRestUtility _restUtility;

        public VehicleInformationService(IRestUtility restUtility)
        {
            _restUtility = restUtility;
        }

        public async Task<VehicleMileageResponse> GetVehicleMileageRating(int vehicleOptionId)
        {
            var mileageQuery = ConstructMileageQuery(vehicleOptionId);

            var mileageResponse = await _restUtility.ExecuteGetRequestAsync<MileageRatingResponse>(mileageQuery);

            return new VehicleMileageResponse
            {
                CityMpg = mileageResponse.City08,
                CombinedMpg = mileageResponse.Comb08,
                HighwayMpg = mileageResponse.Highway08
            };
        }

        private string ConstructMileageQuery(int vehicleOptionId)
        {
            return string.Format(MileageAPI, vehicleOptionId);
        }

        public async Task<VehicleInformationResponse> GetVehicleInformation(string vin)
        {
            //truncate to remove specific vehicle identification characters
            if (vin.Length > 11)
            {
                vin = vin.Substring(0, 11);
            }

            var vinQuery = ConstructVinQuery(vin);

            var vinResponse = await _restUtility.ExecuteGetRequestAsync<VinQueryResponse>(vinQuery);

            var vehicleDetails = new VehicleInformationResponse()
            {
                VIN = vin,
                Make = GetValueFromVinResponse<string>(vinResponse, "Make"),
                SelectedModel = GetValueFromVinResponse<string>(vinResponse, "Model"),
                Year = GetValueFromVinResponse<int>(vinResponse, "Model Year")
            };

            return await GetOptions(vehicleDetails);
        }

        private async Task<VehicleInformationResponse> GetOptions(VehicleInformationResponse vehicleDetails)
        {
            var optionResponse = await GetVehicleOptions(vehicleDetails.Year, vehicleDetails.Make, vehicleDetails.SelectedModel);

            if (optionResponse.MenuItem == null || optionResponse.MenuItem.Count == 0)
            {
                //TODO: possible infinite loop
                //If we got no options, it means the model may be wrong, so we need to send choices back
                var modelOptions = await GetManufacturerModels(vehicleDetails.Year, vehicleDetails.Make);

                vehicleDetails.SelectedModel = null;
                vehicleDetails.VehicleModels = modelOptions.MenuItem.Select(m => m.Value).ToList();
            }
            else if (optionResponse.MenuItem.Count == 1)
            {
                vehicleDetails.SelectedVehicleOption = new VehicleInformationResponseOption
                {
                    Text = optionResponse.MenuItem.First().Text,
                    Id = optionResponse.MenuItem.First().Value
                };

            }
            else
            {
                vehicleDetails.VehicleOptions = optionResponse.MenuItem.Select(m => new VehicleInformationResponseOption
                {
                    Text = m.Text,
                    Id = m.Value
                }).ToList();
            }

            return vehicleDetails;
        }

        private async Task<ModelQueryResponse> GetManufacturerModels(int year, string make)
        {
            var modelsQuery = ConstructModelQuery(year, make);

            var modelsResponse = await _restUtility.ExecuteGetRequestAsync<ModelQueryResponse>(modelsQuery);

            return modelsResponse;
        }

        private string ConstructModelQuery(int year, string make)
        {
            return String.Format(ModelsAPI, year, make);
        }

        private async Task<OptionQueryResponse> GetVehicleOptions(int year, string make, string selectedModel)
        {
            var optionQuery = ConstructOptionQuery(year, make, selectedModel);

            var optionResponse = await _restUtility.ExecuteGetRequestAsync<OptionQueryResponse>(optionQuery);

            return optionResponse;
        }

        internal static string ConstructOptionQuery(int year, string make, string selectedModel)
        {
            return String.Format(OptionsAPI, year, make, selectedModel);
        }

        internal static string ConstructVinQuery(string vin)
        {
            return String.Format(VinAPI, vin);
        }

        internal static T GetValueFromVinResponse<T>(VinQueryResponse response, string key)
        {
            var match = response.Results.Where(r => r.Variable == key).SingleOrDefault();

            if (match == null)
            {
                return default(T);
            }

            var responseValue = (T)Convert.ChangeType(match.Value, typeof(T));

            return responseValue;
        }

        public async Task<VehicleInformationResponse> GetVehicleInformation(int year, string make, string model)
        {
            var vehicleDetails = new VehicleInformationResponse()
            {
                VIN = null,
                Make = make,
                SelectedModel = model,
                Year = year
            };

            return await GetOptions(vehicleDetails);
        }
    }
}