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
        private const double KPHtoMPH = 0.621371;
        private const double AirGramsSecToFuelGalHour = .0874267;

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

        public async Task<double> GetThrottlePercentage()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.ThrottlePercentage)))
            {
                throw new Exception("This vehicle does not support throttle percentage.");
            }

            var throttleReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.ThrottlePercentage);

            if(throttleReading == "NO DATA") //TODO: constant?
            {
                return 0;
            }

            var throttleValue = GetNumericValueFromHexString(throttleReading) * (100.0/255.0);

            return throttleValue;
        }

        public async Task<int> GetInstantMPG()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.MassAirflow)))
            {
                throw new Exception("This vehicle does not support MAF.");
            }

            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.VehicleSpeed)))
            {
                throw new Exception("This vehicle does not support speed.");
            }

            var MAFReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.MassAirflow);

            if (MAFReading == "NO DATA") //TODO: constant?
            {
                return 0;
            }

            var MAFValue = GetNumericValueFromHexString(MAFReading) / 100;

            var speedReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.VehicleSpeed);

            double speedValue = 0;

            if (speedReading != "NO DATA") //TODO: constant?
            {
                speedValue = GetNumericValueFromHexString(speedReading);
            }

            var MPGValue = 7.718 * speedValue / MAFValue;

            return (int)MPGValue;
        }

        private double GetNumericValueFromHexString(string hexString)
        {
            double numericValue = 0;

            var hexChunks = hexString.Length / 2; 

            for(int i = 0; i < hexChunks; i++)
            {
                var currentChunk = hexString.Substring((hexString.Length - 2));

                numericValue += ((double)Convert.ToInt32("0x" + currentChunk, 16)) * (Math.Pow(256, i));

                hexString = hexString.Substring(0, hexString.Length - 2);
            }

            return numericValue;
        }

        public async Task<double> GetMPH()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }
            
            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.VehicleSpeed)))
            {
                throw new Exception("This vehicle does not support speed.");
            }
                        
            var speedReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.VehicleSpeed);

            double speedValue = 0;

            if (speedReading != "NO DATA") //TODO: constant?
            {
                speedValue = GetNumericValueFromHexString(speedReading);
            }
            
            return speedValue * KPHtoMPH; //convert to MPH, natively in KPH
        }

        public async Task<double> GetGPH()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            if (!(await _communicationService.CheckParameterSupported(DiagnosticPIDs.MassAirflow)))
            {
                throw new Exception("This vehicle does not support MAF.");
            }
                        
            var MAFReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.MassAirflow);

            if (MAFReading == "NO DATA") //TODO: constant?
            {
                return 0;
            }

            var MAFValue = GetNumericValueFromHexString(MAFReading) / 100;

            var convertedGPH = MAFValue * AirGramsSecToFuelGalHour;
            
            return convertedGPH;
        }
    }
}
