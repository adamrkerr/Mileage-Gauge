using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IVehicleHistoryService
    {
        Task<IEnumerable<VehicleModel>> GetVehicleHistory();

        Task AddOrUpdateVehicle(VehicleModel newVehicle);

        Task RemoveVehicle(string vin);

    }
}
