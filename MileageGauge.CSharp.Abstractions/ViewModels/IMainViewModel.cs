using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ViewModels
{
    public interface IMainViewModel
    {
        Action<GetDiagnosticDeviceResponse> GetDiagnosticDeviceComplete { get; set; }

        Action<LoadVehicleDetailsResponse> LoadVehicleDetailsComplete { get; set; }

        Func<List<OptionQueryResponseItem>, Task<int>> SelectVehicleOptionCallback { get; set; }

        Task GetDiagnosticDevice();

        Task LoadVehicleDetails(bool forceRefresh);

        IVehicleViewModel CurrentVehicle { get; }

        bool DiagnosticDeviceConnected { get; }
    }
}