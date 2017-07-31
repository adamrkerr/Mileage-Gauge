using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using MileageGauge.CSharp.Abstractions.Services.ELM327;

namespace MileageGauge.ELM327.Implementation
{
    class DemoELM327CommunicationService : IELM327CommunicationService
    {
        private Dictionary<string, Queue<string>> _sampleParameterValues;

        private async Task LoadSampleData()
        {
            _sampleParameterValues = new Dictionary<string, Queue<string>>();

            var fileAsString = new StringBuilder();

            using (var sampleStream = Application.Context.Assets.Open("demo/morning_drive.txt"))
            {
                var buffer = new byte[100];

                while ((await sampleStream.ReadAsync(buffer, 0, 100)) != 0)
                {
                    fileAsString.Append(Encoding.ASCII.GetString(buffer));
                }
            }

            var fileLines = fileAsString.ToString().Split('\n');

            foreach (var line in fileLines)
            {
                var trimmedLine = line.Trim();

                if (String.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                //Allow us to comment stuff out.
                if (trimmedLine.StartsWith("*"))
                    continue;

                if (trimmedLine.Length == 4 || trimmedLine.Length == 2) //2 for reading trouble codes
                {
                    if (!_sampleParameterValues.ContainsKey(trimmedLine))
                    {
                        _sampleParameterValues.Add(trimmedLine, new Queue<string>());
                    }
                }
                else if (trimmedLine.Length > 4)
                {
                    string pid;

                    if (trimmedLine.StartsWith("41"))
                    {
                        //get first 5
                        pid = trimmedLine.Substring(0, 5);
                    }
                    else
                    {
                        //get first 3, trouble codes
                        pid = trimmedLine.Substring(0, 3);
                    }

                    pid = pid.Replace(" ", string.Empty);

                    string key, responseValue;

                    if (trimmedLine.StartsWith("41"))
                    {
                        key = "0" + pid.Substring(1, 3); //replace leading 4 with 0 4111 -> 0111
                        responseValue = trimmedLine.Substring(5).Trim().Replace(" ", string.Empty);
                    }
                    else
                    {
                        key = "0" + pid.Substring(1, 1); //replace leading 4 with 0 43 -> 03
                        responseValue = trimmedLine.Substring(3).Trim().Replace(" ", string.Empty);
                    }

                    if (_sampleParameterValues.ContainsKey(key))
                    {
                        _sampleParameterValues[key].Enqueue(responseValue);
                    }
                }
            }
        }

        public Task<bool> CheckParameterSupported(DiagnosticPIDs pid)
        {
            return Task.FromResult(true);
        }

        public async Task<ConnectionResponse> Connect(string deviceAddress)
        {
            //load the sample data
            await LoadSampleData();

            return new ConnectionResponse() { Success = true };
        }

        public async Task<string> GetVehicleParameterValue(DiagnosticPIDs pid)
        {
            switch (pid)
            {
                case DiagnosticPIDs.GetVIN:
                    return await GetRandomVin();
                case DiagnosticPIDs.MassAirflow:
                case DiagnosticPIDs.ThrottlePercentage:
                case DiagnosticPIDs.VehicleSpeed:
                case DiagnosticPIDs.GetSupportedPIDs:
                    return await GetNextValue(pid);
                default:
                    return await Task.FromResult("NO DATA");
            }

        }

        private async Task<string> GetNextValue(DiagnosticPIDs pid)
        {
            await Task.Delay(200);

            string pidCode = String.Format("{0:X}", pid).Substring(4);
            
            if (!_sampleParameterValues.ContainsKey(pidCode))
                return await Task.FromResult("NO DATA");

            var nextValue = _sampleParameterValues[pidCode].Dequeue();

            //roll this back onto the end of the queue so it keeps going

            _sampleParameterValues[pidCode].Enqueue(nextValue);

            return await Task.FromResult(nextValue);
        }

        private async Task<string> GetRandomVin()
        {
            await Task.Delay(1000);

            //TODO: get this from the ELM327
            Random rnd = new Random();
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

        public void Dispose()
        {
            //do nothing, this is a demo
        }

        public async Task ClearDiagnosticCodes()
        {
            await Task.Delay(200);

            var diagnosticPid = "03";

            var nextDiagnosticResponse = _sampleParameterValues[diagnosticPid].Peek();

            //cycle until the "all clear" record is next
            while(!String.IsNullOrEmpty(nextDiagnosticResponse.Replace("0", string.Empty).Trim()))
            {
                var nextValue = _sampleParameterValues[diagnosticPid].Dequeue();

                //roll this back onto the end of the queue so it keeps going

                _sampleParameterValues[diagnosticPid].Enqueue(nextValue);

                nextDiagnosticResponse = _sampleParameterValues[diagnosticPid].Peek();
            }
            
        }

        private const int DiagnosticPID = 0x03;

        public async Task<string> GetDiagnosticCodes()
        {
            await Task.Delay(200);

            string pidCode = String.Format("{0:X}", DiagnosticPID).Substring(6);


            if (!_sampleParameterValues.ContainsKey(pidCode))
                return await Task.FromResult("NO DATA");

            var nextValue = _sampleParameterValues[pidCode].Dequeue();

            //roll this back onto the end of the queue so it keeps going

            _sampleParameterValues[pidCode].Enqueue(nextValue);

            return await Task.FromResult(nextValue);
        }
    }
}