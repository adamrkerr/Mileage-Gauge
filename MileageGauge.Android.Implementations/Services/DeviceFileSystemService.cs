using System;
using MileageGauge.CSharp.Abstractions.Services;

namespace MileageGauge.Android.Implementations.Services
{
    public class DeviceFileSystemService : IDeviceFileSystemService
    {
        public string GetApplicationFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}