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

        Task GetDiagnosticDevice(string deviceAddress);
                
        VehicleModel CurrentVehicle { get; set; }

        String DiagnosticDeviceAddress { get; set; }

        Task<IEnumerable<VehicleModel>> GetVehicleHistory();
        
    }
}