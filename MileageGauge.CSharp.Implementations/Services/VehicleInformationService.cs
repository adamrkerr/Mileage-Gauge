using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Implementations.ViewModels;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Linq;
using System.Collections.Generic;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class VehicleInformationService : IVehicleInformationService
    {
        //TODO: move this somewhere else?
        private const string VinAPI = "https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVin/{0}*?format=json";
        private const string OptionsAPI = "https://fueleconomy.gov/ws/rest/vehicle/menu/options?year={0}&make={1}&model={2}";

        private readonly IRestUtility _restUtility;

        public VehicleInformationService(IRestUtility restUtility)
        {
            _restUtility = restUtility;
        }

        public async Task<IVehicleViewModel> GetVehicleInformation(Func<List<OptionQueryResponseItem>, Task<int>> selectVehicleOptionCallback)
        {
            var vin = await GetVehicleVIN();

            var vinQuery = ConstructVinQuery(vin);

            var vinResponse = await _restUtility.ExecuteGetRequestAsync<VinQueryResponse>(vinQuery);

            var vehicleDetails = new VehicleViewModel()
            {
                VIN = vin,
                Make = GetValueFromVinResponse<string>(vinResponse, "Make"),
                Model = GetValueFromVinResponse<string>(vinResponse, "Model"),
                Year = GetValueFromVinResponse<int>(vinResponse, "Model Year")
                //Option = "Man 6-spd, 6 cyl, 3.2 L",
                //CityMPG = 15,
                //HighwayMPG = 23,
                //CombinedMPG = 18

            };

            var optionQuery = ConstructOptionQuery(vehicleDetails);

            var optionResponse = await _restUtility.ExecuteGetRequestAsync<OptionQueryResponse>(optionQuery);

            if(optionResponse.MenuItem.Count == 1)
            {
                vehicleDetails.Option = optionResponse.MenuItem.First().Text;
            }
            else
            {
                var selectedOption = await selectVehicleOptionCallback?.Invoke(optionResponse.MenuItem);
            }

            return vehicleDetails;
        }

        internal static string ConstructOptionQuery(VehicleViewModel vehicleDetails)
        {
            return String.Format(OptionsAPI, vehicleDetails.Year, vehicleDetails.Make, vehicleDetails.Model);
        }

        internal static string ConstructVinQuery(string vin)
        {
            if(vin.Length > 11)
            {
                vin = vin.Substring(0, 11);
            }

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

        private async Task<string> GetVehicleVIN()
        {
            //TODO: get this from the ELM327
            return await Task.FromResult("1C3AN69L24X12345");
        }
    }
}