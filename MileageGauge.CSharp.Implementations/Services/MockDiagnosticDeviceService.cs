using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class MockDiagnosticDeviceService : IDiagnosticDeviceService
    {
        public bool IsConnected
        {
            get; private set;
        }

        public async Task<bool> Connect()
        {
            await Task.Delay(5000);

            IsConnected = true;

            return true;
        }

        public void Dispose()
        {
            
        }

        public async Task<string> GetVin()
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
