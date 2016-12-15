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
        Task<GetDiagnosticDeviceResponse> GetDiagnosticDevice();

        Task<LoadVehicleDetailsResponse> LoadVehicleDetails(bool forceRefresh);

        IVehicleViewModel CurrentVehicle { get; }

        bool DiagnosticDeviceConnected { get; }
    }
}