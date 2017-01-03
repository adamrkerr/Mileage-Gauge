using MileageGauge.CSharp.Abstractions.ResponseModels;
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

        Task GetDiagnosticDevice();

        Task LoadVehicleDetails(bool forceRefresh);

        IVehicleViewModel CurrentVehicle { get; }

        bool DiagnosticDeviceConnected { get; }
    }
}