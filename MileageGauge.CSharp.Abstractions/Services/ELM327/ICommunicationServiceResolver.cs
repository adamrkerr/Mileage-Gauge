using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ELM327
{
    public interface ICommunicationServiceResolver
    {
        IELM327CommunicationService ResolveCommunicationService(string bluetoothDeviceAddress);
    }
}
