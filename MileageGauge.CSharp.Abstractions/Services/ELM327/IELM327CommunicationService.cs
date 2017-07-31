using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ELM327
{
    public interface IELM327CommunicationService : IDisposable
    {
        Task<ConnectionResponse> Connect(string deviceAddress);

        Task<string> GetVehicleParameterValue(DiagnosticPIDs pid);

        Task<bool> CheckParameterSupported(DiagnosticPIDs pid);

        Task ClearDiagnosticCodes();
    }
}
