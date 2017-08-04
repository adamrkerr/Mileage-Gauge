using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System.IO;
using Newtonsoft.Json;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class VehicleHistoryService : IVehicleHistoryService
    {
        private const string VEHICLE_HISTORY_FILE = "vehicle_history.txt";

        private readonly IDeviceFileSystemService _fileSystemService;
        private readonly ITripHistoryService _tripService;

        public VehicleHistoryService(IDeviceFileSystemService fileSystemService, ITripHistoryService tripService)
        {
            _fileSystemService = fileSystemService;
            _tripService = tripService;
        }
        
        public async Task AddOrUpdateVehicle(VehicleModel newVehicle)
        {
            var currentCollection = (await GetVehicleHistory()).ToList();

            var match = currentCollection.SingleOrDefault(s => s.VIN == newVehicle.VIN);

            newVehicle.LastSelected = DateTime.UtcNow; //force this to the top

            if(match != null)
            {
                match.Year = newVehicle.Year;
                match.Make = newVehicle.Make;
                match.Option = newVehicle.Option;
                match.CityMPG = newVehicle.CityMPG;
                match.HighwayMPG = newVehicle.HighwayMPG;
                match.CombinedMPG = newVehicle.CombinedMPG;
                match.LastSelected = newVehicle.LastSelected;
            }
            else
            {
                currentCollection.Add(newVehicle);
            }

            await SaveVehicleHistory(currentCollection);
        }

        public async Task<IEnumerable<VehicleModel>> GetVehicleHistory()
        {
            var folderPath = _fileSystemService.GetApplicationFolder();

            var filePath = Path.Combine(folderPath, VEHICLE_HISTORY_FILE);

            using (var file = File.Open(filePath, FileMode.OpenOrCreate))
            using (var streamReader = new StreamReader(file))
            {
                var fileText = await streamReader.ReadToEndAsync();

                if (String.IsNullOrEmpty(fileText))
                    return new List<VehicleModel>();

                var collection = JsonConvert.DeserializeObject<List<VehicleModel>>(fileText);

                return collection;
            }
        }

        public async Task RemoveVehicle(string vin)
        {
            var currentCollection = (await GetVehicleHistory()).ToList();

            var match = currentCollection.SingleOrDefault(s => s.VIN == vin);
                        
            if (match != null)
            {
                currentCollection.Remove(match);
            }

            await SaveVehicleHistory(currentCollection);

            await _tripService.RemoveTripHistoryForVehicle(vin);
        }

        private async Task SaveVehicleHistory(List<VehicleModel> collection)
        {
            var folderPath = _fileSystemService.GetApplicationFolder();

            var filePath = Path.Combine(folderPath, VEHICLE_HISTORY_FILE);

            using (var file = File.Open(filePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(file))
            {
                var fileText = JsonConvert.SerializeObject(collection);

                await streamWriter.WriteAsync(fileText);
            }
        }
    }
}
