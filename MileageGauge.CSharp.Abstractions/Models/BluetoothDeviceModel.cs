using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Models
{
    public class BluetoothDeviceModel
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public const string DemoAddress = "-1";

        public const string DemoName = "Demo";

        public static BluetoothDeviceModel GetDemoDevice()
        {
            return new BluetoothDeviceModel() { Address = DemoAddress, Name = DemoName };
        }
    }
}
