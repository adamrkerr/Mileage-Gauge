using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ELM327
{
    public enum DiagnosticPIDs
    {
        Unknown = 0,
        GetSupportedPIDs1 = 0x0100,
        MonitorStatus = 0x0101,
        FreezeDTC = 0x0102,
        FuelSystemStatus = 0x0103,
        EngineLoad = 0x0104,
        CoolantTemperature = 0x0105,
        ShortFuelTrim1 = 0x0106,
        LongFuelTrim1 = 0x0107,
        ShortFuelTrim2 = 0x0108,
        LongFuelTrim2 = 0x0109,
        FuelPressure = 0x010A,
        ManifoldPressure = 0x010B,
        EngineRPM = 0x010C,
        VehicleSpeed = 0x010D,
        TimingAdvance = 0x010E,
        IntakeAirTemperature = 0x010F,
        MassAirflowRate = 0x0110,
        ThrottlePercentage = 0x0111,
        CommandedSecondaryAir = 0x0112,
        O2Present2 = 0x0113,
        O2Voltage1 = 0x0114,
        O2Voltage2 = 0x0115,
        O2Voltage3 = 0x0116,
        O2Voltage4 = 0x0117,
        O2Voltage5 = 0x0118,
        O2Voltage6 = 0x0119,
        O2Voltage7 = 0x011A,
        O2Voltage8 = 0x011B,
        OBDStandard = 0x011C,
        O2Present4 = 0x011D,
        AuxInputStatus = 0x011E,
        EngineRunTime = 0x011F,
        GetSupportedPIDs2 = 0x0120,
        GetVIN = 0x0902,
    }
}
