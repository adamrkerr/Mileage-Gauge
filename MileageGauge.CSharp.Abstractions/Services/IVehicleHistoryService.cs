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
        Task<IEnumerable<IVehicleViewModel>> GetVehicleHistory();

        Task AddNewVehicle(IVehicleViewModel newVehicle);

        Task RemoveVehicle(Guid internalId);

    }
}
