using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IDiagnosticDeviceService : IDisposable
    {
        Task<bool> Connect(string deviceAddress);

        bool IsConnected { get; }

        Task<string> GetVin();

        Task<double> GetThrottlePercentage();

        Task<double> GetMPH();

        Task<double> GetGPH();
    }
}
