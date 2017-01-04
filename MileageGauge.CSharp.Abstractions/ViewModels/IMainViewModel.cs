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

        Action<LoadVehicleDetailsCompleteResponse> LoadVehicleDetailsComplete { get; set; }

        Action<LoadVehicleDetailsOptionRequiredResponse> LoadVehicleDetailsOptionsRequired { get; set; }

        Task GetDiagnosticDevice();

        Task LoadVehicleDetails(bool forceRefresh);

        Task CompleteVehicleDetails(VehicleOptionViewModel selectedOption);

        Task ContinueWithoutVehicleDetails();

        IVehicleViewModel CurrentVehicle { get; }

        bool DiagnosticDeviceConnected { get; }
    }
}