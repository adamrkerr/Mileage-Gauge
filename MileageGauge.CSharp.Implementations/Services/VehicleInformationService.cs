using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Implementations.ViewModels;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class VehicleInformationService : IVehicleInformationService
    {
        //TODO: move this somewhere else?
        private const string VinAPI = "https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVin/{0}*?format=json";

        private readonly IRestUtility _restUtility;

        public VehicleInformationService(IRestUtility restUtility)
        {
            _restUtility = restUtility;
        }

        public async Task<IVehicleViewModel> GetVehicleInformation()
        {
            var vin = await GetVehicleVIN();

            /*
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
             */

            var query = ConstructVinQuery(vin);

            var vinResult = await _restUtility.ExecuteGetRequestAsync(query);

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

            return vehicleDetails;
        }

        internal static string ConstructVinQuery(string vin)
        {
            if(vin.Length > 11)
            {
                vin = vin.Substring(0, 11);
            }

            return String.Format(VinAPI, vin);
        }

        private async Task<string> GetVehicleVIN()
        {
            //TODO: get this from the ELM327
            return await Task.FromResult("1C3AN69L24X12345");
        }
    }
}