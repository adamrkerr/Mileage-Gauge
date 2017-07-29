using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ViewModels;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class VehicleHistoryService : IVehicleHistoryService
    {
        public Task AddNewVehicle(IVehicleViewModel newVehicle)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IVehicleViewModel>> GetVehicleHistory()
        {
            throw new NotImplementedException();
        }

        public Task RemoveVehicle(Guid internalId)
        {
            throw new NotImplementedException();
        }
    }
}
