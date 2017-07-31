using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.Services.ELM327;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class DiagnosticDeviceService : IDiagnosticDeviceService
    {
        private const double KPHtoMPH = 0.621371;
        private const double AirGramsSecToFuelGalHour = .0874267;

        private IELM327CommunicationService _communicationService;

        private HashSet<DiagnosticPIDs> _supportedPIDs;

        public bool IsConnected
        {
            get; private set;
        }

        private readonly ICommunicationServiceResolver _serviceResolver;

        public DiagnosticDeviceService(ICommunicationServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
            _supportedPIDs = new HashSet<DiagnosticPIDs>();
        }

        public async Task<bool> Connect(string deviceAddress)
        {
            if (IsConnected)
                return IsConnected;

            _communicationService = _serviceResolver.ResolveCommunicationService(deviceAddress);

            var connectionResponse = await _communicationService.Connect(deviceAddress);            

            //TODO: it would be good to do something with the message if failed

            IsConnected = connectionResponse.Success;

            if (IsConnected)
            {
                await LoadSupportedPIDs();
            }

            return IsConnected;
        }

        private async Task LoadSupportedPIDs()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }
                        
            var supportedString = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.GetSupportedPIDs1);

            if (supportedString == "NO DATA") //TODO: constant?
            {
                return;
            }

            var intValue = Convert.ToUInt32(GetNumericValueFromHexString(supportedString));

            var supportedBinary = Convert.ToString(intValue, toBase:2);

            const int mode1Offset = 0x0100; //add this to each positive value to find the key

            for(int i = 0; i < supportedBinary.Length; i++)
            {
                var currentPosition = supportedBinary[i];

                if (currentPosition == '0')
                    continue;

                var currentPIDValue = mode1Offset + (i + 1);

                var currentPID = (DiagnosticPIDs) currentPIDValue;

                _supportedPIDs.Add(currentPID);
            }

        }

        public void Dispose()
        {
            if(_communicationService != null)
                _communicationService.Dispose();
        }

        public async Task<string> GetVin()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            return await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.GetVIN);
        }

        public async Task<double> GetThrottlePercentage()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            if (!_supportedPIDs.Contains(DiagnosticPIDs.ThrottlePercentage))
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

            if (!_supportedPIDs.Contains(DiagnosticPIDs.MassAirflowRate))
            {
                throw new Exception("This vehicle does not support MAF.");
            }

            if (!_supportedPIDs.Contains(DiagnosticPIDs.VehicleSpeed))
            {
                throw new Exception("This vehicle does not support speed.");
            }

            var MAFReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.MassAirflowRate);

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
            
            if (!_supportedPIDs.Contains(DiagnosticPIDs.VehicleSpeed))
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

            if (!_supportedPIDs.Contains(DiagnosticPIDs.MassAirflowRate))
            {
                throw new Exception("This vehicle does not support MAF.");
            }
                        
            var MAFReading = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.MassAirflowRate);

            if (MAFReading == "NO DATA") //TODO: constant?
            {
                return 0;
            }

            var MAFValue = GetNumericValueFromHexString(MAFReading) / 100;

            var convertedGPH = MAFValue * AirGramsSecToFuelGalHour;
            
            return convertedGPH;
        }

        public async Task<IEnumerable<string>> GetDiagnosticCodes()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            var rawErrorCodeString = await _communicationService.GetDiagnosticCodes();

            var individualRawCodes = new List<string>();

            //all zeroes means no errors
            if (String.IsNullOrEmpty(rawErrorCodeString.Replace("0", String.Empty)))
            {
                return individualRawCodes;
            }

            for(int startIndex = 0; startIndex < rawErrorCodeString.Length; startIndex += 4)
            {
                individualRawCodes.Add(rawErrorCodeString.Substring(startIndex, 4));
            }

            var formattedCodes = new List<string>();

            foreach(var rawCode in individualRawCodes)
            {
                if (String.IsNullOrEmpty(rawCode.Replace("0", String.Empty)))
                    continue;

                formattedCodes.Add(FormatRawDiagnosticCode(rawCode));
            }

            return formattedCodes;
        }

        private static string FormatRawDiagnosticCode(string rawCode)
        {            
            var firstChar = rawCode.Substring(0, 1);

            string prefix;

            switch (firstChar)
            {
                case "0":
                    prefix = "P0";
                    break;
                case "1":
                    prefix = "P1";
                    break;
                case "2":
                    prefix = "P2";
                    break;
                case "3":
                    prefix = "P3";
                    break;
                case "4":
                    prefix = "C0";
                    break;
                case "5":
                    prefix = "C1";
                    break;
                case "6":
                    prefix = "C2";
                    break;
                case "7":
                    prefix = "C3";
                    break;
                case "8":
                    prefix = "B0";
                    break;
                case "9":
                    prefix = "B1";
                    break;
                case "A":
                    prefix = "B2";
                    break;
                case "B":
                    prefix = "B3";
                    break;
                case "C":
                    prefix = "U0";
                    break;
                case "D":
                    prefix = "U1";
                    break;
                case "E":
                    prefix = "U2";
                    break;
                case "F":
                    prefix = "U3";
                    break;
                default:
                    prefix = "00";
                    break;
            }                      

            return $"{prefix}{rawCode.Substring(1)}";
        }

        public async Task ClearDiagnosticCodes()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            await _communicationService.ClearDiagnosticCodes();
        }
    }
}
