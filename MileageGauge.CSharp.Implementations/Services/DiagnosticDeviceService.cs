using MileageGauge.CSharp.Abstractions.Models;
using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.Services.ELM327;
using MileageGauge.ELM327.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class DiagnosticDeviceService : IDiagnosticDeviceService
    {
        private IELM327CommunicationService _communicationService;

        public bool IsConnected
        {
            get; private set;
        }

        public async Task<bool> Connect(string deviceAddress)
        {
            if (IsConnected)
                return IsConnected;

            if(deviceAddress == BluetoothDeviceModel.DemoAddress)
            {
                _communicationService = new DemoELM327CommunicationService();
            }
            else
            {

            }

            var connectionResponse = await _communicationService.Connect(deviceAddress);            

            //TODO: it would be good to do something with the message if failed

            IsConnected = connectionResponse.Success;

            return IsConnected;
        }

        public void Dispose()
        {
            
        }

        public async Task<string> GetVin()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.GetVIN)))
            {
                //TODO: prompt user for manual VIN
                throw new Exception("This vehicle does not support VIN retrieval.");
            }

            return await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.GetVIN);
        }
    }
}
