using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content;

namespace MileageGauge.Diagnostic.Implementation
{
    public class DiagnosticDeviceService : IDiagnosticDeviceService
    {
        public bool IsConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<bool> Connect(string deviceAddress)
        {
            var adapter = BluetoothAdapter.DefaultAdapter;

            return Task.FromResult(false);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVin()
        {
            throw new NotImplementedException();
        }
    }
}
