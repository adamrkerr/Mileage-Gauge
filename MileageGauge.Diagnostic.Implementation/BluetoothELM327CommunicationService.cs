using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Services.ELM327;
using Android.Bluetooth;
using Java.Util;
using System.IO;

namespace MileageGauge.ELM327.Implementation
{
    class BluetoothELM327CommunicationService : IELM327CommunicationService
    {
        private BluetoothSocket _diagnosticSocket;
        private static UUID SERIAL_UUID = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
        private const char RESPONSE_END = '>';

        public async Task<bool> CheckParameterSupported(DiagnosticPIDs pid)
        {
            //TODO: check this from the device

            return await Task.FromResult(true);
        }

        public async Task<ConnectionResponse> Connect(string deviceAddress)
        {
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            //Both of these should already be handled upstream
            if (bluetoothAdapter == null)
                return new ConnectionResponse { Success = false, Message = "This device does not support Bluetooth." };

            if (!bluetoothAdapter.IsEnabled)
                return new ConnectionResponse { Success = false, Message = "Bluetooth is not currently enabled." };

            var diagnosticDevice = bluetoothAdapter.BondedDevices.SingleOrDefault(d => d.Address == deviceAddress);

            if (diagnosticDevice == null)
                return new ConnectionResponse { Success = false, Message = "Bluetooth device not found at the specified address." };

            _diagnosticSocket = diagnosticDevice.CreateRfcommSocketToServiceRecord(SERIAL_UUID);

            try
            {
                await _diagnosticSocket.ConnectAsync();
            }
            catch(Java.IO.IOException ex)
            {
                return new ConnectionResponse { Success = false, Message = ex.Message };
            }

            return new ConnectionResponse { Success = true };
        }

        public void Dispose()
        {
            if (_diagnosticSocket != null)
                _diagnosticSocket.Dispose();
        }

        public async Task<string> GetVehicleParameterValue(DiagnosticPIDs pid)
        {
            //TODO: just for demo, remove this when this functionality actually works
            if(pid == DiagnosticPIDs.GetVIN)
            {
                return await GetRandomVin();
            }

            var pidCode = String.Format("{0:X}1\r", pid).Substring(4);

            await _diagnosticSocket.OutputStream.WriteAsync(pidCode.Select(c => (byte)c).ToArray(), 0, pidCode.Length);

            await _diagnosticSocket.OutputStream.FlushAsync();

            return await GetResponseFromStream(_diagnosticSocket.InputStream);            
        }

        private async Task<string> GetResponseFromStream(Stream inputStream)
        {
            var builder = new StringBuilder();

            var buffer = new byte[1];

            await inputStream.ReadAsync(buffer, 0, 1);

            var responseChar = (char)buffer[0];

            while(responseChar != RESPONSE_END)
            {
                builder.Append(responseChar);

                await inputStream.ReadAsync(buffer, 0, 1);

                responseChar = (char)buffer[0];
            }

            var responseString = builder.ToString();

            var commandAndResponse = responseString.Trim().Split('\r');

            if(commandAndResponse.Length != 2)
            {
                return "00";
            }

            var responseOnly = commandAndResponse[1];

            responseOnly = responseOnly
                                .Substring(5) //First 5 chars are just the command pid
                                .Trim()
                                .TrimEnd('\0')
                                .Replace("\r", string.Empty)
                                .Replace("\n", string.Empty)
                                .Replace(" ", string.Empty);

            return responseOnly;
        }

        //TODO: from demo class, remove when this is actually implemented
        private async Task<string> GetRandomVin()
        {
            await Task.Delay(1000);

            //TODO: get this from the ELM327
            var rnd = new System.Random();
            int choice = rnd.Next(1, 5);

            switch (choice)
            {
                case 1:
                    return await Task.FromResult("1C3AN69L24X12345");
                case 2:
                    return await Task.FromResult("JHMAP21446S12345");
                case 3:
                    return await Task.FromResult("1G4PS5SK2G4142345");
                case 4:
                    return await Task.FromResult("WDBBA48D2GA051234");
            }
            return await Task.FromResult("impossible?");
        }

        private const string ResetDiagnosticPID = "04";

        public async Task ClearDiagnosticCodes()
        {
            var pidCode = $"{ResetDiagnosticPID}\r";

            await _diagnosticSocket.OutputStream.WriteAsync(pidCode.Select(c => (byte)c).ToArray(), 0, pidCode.Length);

            await _diagnosticSocket.OutputStream.FlushAsync();

            await GetResponseFromStream(_diagnosticSocket.InputStream);
        }

        private const string DiagnosticPID = "03";

        public async Task<string> GetDiagnosticCodes()
        {
            var pidCode = $"{DiagnosticPID}\r";

            await _diagnosticSocket.OutputStream.WriteAsync(pidCode.Select(c => (byte)c).ToArray(), 0, pidCode.Length);

            await _diagnosticSocket.OutputStream.FlushAsync();

            return await GetResponseFromStream(_diagnosticSocket.InputStream);
        }
    }
}