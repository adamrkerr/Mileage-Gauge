using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MileageGauge.Abstractions.ResponseModels;
using System.Threading.Tasks;

namespace MileageGauge.Abstractions
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