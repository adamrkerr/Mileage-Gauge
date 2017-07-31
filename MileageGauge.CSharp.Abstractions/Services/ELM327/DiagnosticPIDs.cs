using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ELM327
{
    public enum DiagnosticPIDs
    {
        ThrottlePercentage = 0x0111,
        VehicleSpeed = 0x010D,
        MassAirflow = 0x0110,
        GetVIN = 0x9999,
        GetDiagnosticCodes = 0x03,
        GetSupportedPIDs = 0x0100
    }
}
