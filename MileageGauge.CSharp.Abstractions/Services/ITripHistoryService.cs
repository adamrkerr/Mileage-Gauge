using MileageGauge.CSharp.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface ITripHistoryService
    {
        Task<TripHistory> GetTripHistory(string vehicleVin);

        Task AddOrUpdateTripHistory(TripHistory history);

        Task RemoveTripHistoryForVehicle(string vin);
    }
}
