using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Models;
using System.IO;
using Newtonsoft.Json;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class TripHistoryService : ITripHistoryService
    {
        private const string TRIP_HISTORY_FILE = "trip_history.txt";

        private readonly IDeviceFileSystemService _fileSystemService;

        public TripHistoryService(IDeviceFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public async Task AddOrUpdateTripHistory(TripHistory history)
        {
            var histories = await GetAllTripHistories().ConfigureAwait(false);

            var trip = histories.SingleOrDefault(h => h.Vin == history.Vin);

            if (trip == null)
            {
                histories.Add(history);
            }
            else
            {
                trip.GallonsUsed = history.GallonsUsed;
                trip.MilesTravelled = history.MilesTravelled;
            }

            await SaveHistories(histories).ConfigureAwait(false);
        }

        private async Task SaveHistories(List<TripHistory> histories)
        {
            var folderPath = _fileSystemService.GetApplicationFolder();

            var filePath = Path.Combine(folderPath, TRIP_HISTORY_FILE);

            using (var file = File.Open(filePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(file))
            {
                var fileText = JsonConvert.SerializeObject(histories);

                await streamWriter.WriteAsync(fileText).ConfigureAwait(false);
            }
        }

        public async Task<TripHistory> GetTripHistory(string vehicleVin)
        {
            var histories = await GetAllTripHistories().ConfigureAwait(false);

            var trip = histories.SingleOrDefault(h => h.Vin == vehicleVin);

            return trip;
        }

        private async Task<List<TripHistory>> GetAllTripHistories()
        {
            var folderPath = _fileSystemService.GetApplicationFolder();

            var filePath = Path.Combine(folderPath, TRIP_HISTORY_FILE);

            using (var file = File.Open(filePath, FileMode.OpenOrCreate))
            using (var streamReader = new StreamReader(file))
            {
                var fileText = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                if (String.IsNullOrEmpty(fileText))
                    return new List<TripHistory>();

                var collection = JsonConvert.DeserializeObject<List<TripHistory>>(fileText);

                return collection;

            }
        }

        public async Task RemoveTripHistoryForVehicle(string vin)
        {
            var histories = await GetAllTripHistories().ConfigureAwait(false);

            var trip = histories.SingleOrDefault(h => h.Vin == vin);

            if (trip == null)
            {
                return;
            }
            else
            {
                histories.Remove(trip);
                await SaveHistories(histories).ConfigureAwait(false);
            }
        }
    }
}
