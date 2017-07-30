using System;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services;
using System.Collections.Generic;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Linq;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private readonly IDiagnosticDeviceService _diagnosticService;
        private readonly IVehicleHistoryService _vehicleHistoryService;

        public MainViewModel(IDiagnosticDeviceService diagnosticService, IVehicleHistoryService vehicleHistoryService)
        {
            _diagnosticService = diagnosticService;
            _vehicleHistoryService = vehicleHistoryService;
        }

        public VehicleModel CurrentVehicle
        {
            get; private set;
        }

        public Action<GetDiagnosticDeviceResponse> GetDiagnosticDeviceComplete
        {
            get; set;
        }

        public string DiagnosticDeviceAddress
        {
            get;set;
        }

        public async Task GetDiagnosticDevice(string deviceAddress)
        {
            var connected = await _diagnosticService.Connect(deviceAddress);

            DiagnosticDeviceAddress = deviceAddress;

            //TODO: handle failure

            GetDiagnosticDeviceComplete?.Invoke(new GetDiagnosticDeviceResponse { Success = connected, DeviceAddress = deviceAddress });
        }

        public async Task<IEnumerable<VehicleModel>> GetVehicleHistory()
        {
            return await _vehicleHistoryService.GetVehicleHistory();
        }

        public async Task RemoveVehicle(string vin)
        {
            await _vehicleHistoryService.RemoveVehicle(vin);
        }

        public async Task SetCurrentVehicle(VehicleModel vehicle)
        {
            vehicle.LastSelected = DateTime.UtcNow;

            await _vehicleHistoryService.AddOrUpdateVehicle(vehicle);

            CurrentVehicle = vehicle;
        }
    }
}