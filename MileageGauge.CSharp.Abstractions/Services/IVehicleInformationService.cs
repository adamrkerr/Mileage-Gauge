using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IVehicleInformationService
    {
        Task<IVehicleViewModel> GetVehicleInformation(Func<List<OptionQueryResponseItem>, Task<int>> selectVehicleOptionCallback);
    }
}