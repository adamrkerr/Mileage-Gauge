using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IDiagnosticDeviceService : IDisposable
    {
        Task<bool> Connect();

        bool IsConnected { get; }

        Task<string> GetVin();

    }
}
