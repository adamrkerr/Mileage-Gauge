using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MileageGauge.CSharp.Abstractions.Services.ELM327;
using System.IO;

namespace MileageGauge.ELM327.Implementation
{
    public class DemoELM327CommunicationService : IELM327CommunicationService
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
                default:
                    return await Task.FromResult("NO DATA");
            }

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
    }
}