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

        public bool IsConnected
        {
            get; private set;
        }

        private readonly ICommunicationServiceResolver _serviceResolver;

        public DiagnosticDeviceService(ICommunicationServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public async Task<bool> Connect(string deviceAddress)
        {
            if (IsConnected)
                return IsConnected;

            _communicationService = _serviceResolver.ResolveCommunicationService(deviceAddress);

            var connectionResponse = await _communicationService.Connect(deviceAddress);            

            //TODO: it would be good to do something with the message if failed

            IsConnected = connectionResponse.Success;

            return IsConnected;
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

        public async Task<IEnumerable<string>> GetDiagnosticCodes()
        {
            if (!IsConnected)
            {
                throw new Exception("Device is not connected");
            }

            var rawErrorCodeString = await _communicationService.GetVehicleParameterValue(DiagnosticPIDs.GetDiagnosticCodes);

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
            //first, convert this string from foru alpha characters to two bytes
            var firstByte = Convert.ToByte(rawCode.Substring(0, 2));
            var secondByte = Convert.ToByte(rawCode.Substring(2, 2));

            //then, convert the int to a binary string
            var binaryString = String.Format("{0}{1}",
                Convert.ToString(firstByte, 2).PadLeft(8, '0'),
                Convert.ToString(secondByte, 2).PadLeft(8, '0'));

            //now, that parsing
            var codeBuilder = new StringBuilder(6);

            var firstPosition = binaryString.Substring(0, 2);

            switch (firstPosition)
            {
                case "00":
                    codeBuilder.Append("P");
                    break;
                case "01":
                    codeBuilder.Append("C");
                    break;
                case "10":
                    codeBuilder.Append("B");
                    break;
                case "11":
                    codeBuilder.Append("N");
                    break;
            }

            var secondPosition = binaryString.Substring(2, 2);

            codeBuilder.Append(Convert.ToInt32(secondPosition, 2));

            codeBuilder.Append(ConvertBinaryStringToHex(binaryString.Substring(4, 4)));

            codeBuilder.Append(ConvertBinaryStringToHex(binaryString.Substring(8, 4)));

            codeBuilder.Append(ConvertBinaryStringToHex(binaryString.Substring(12, 4)));

            return codeBuilder.ToString();
        }

        private static string ConvertBinaryStringToHex(string binaryString)
        {
            var hextString = Convert.ToInt32(binaryString, 2).ToString("X");
            return hextString.Replace("0x", string.Empty);
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
