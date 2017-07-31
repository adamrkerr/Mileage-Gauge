using MileageGauge.CSharp.Abstractions.Services.ELM327;
using MileageGauge.CSharp.Abstractions.Models;

namespace MileageGauge.ELM327.Implementation
{
    /// <summary>
    /// Exists solely to transparently provide a demo implementation based on the supplied device address
    /// </summary>
    public class CommunicationServiceResolver : ICommunicationServiceResolver
    {
        public IELM327CommunicationService ResolveCommunicationService(string bluetoothDeviceAddress)
        {
            if (bluetoothDeviceAddress == BluetoothDeviceModel.DemoAddress)
            {
                return new DemoELM327CommunicationService();
            }
            else
            {
                return new BluetoothELM327CommunicationService();
            }
        }
    }
}