using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MileageGauge.Abstractions;

namespace MileageGauge.Implementations
{
    public class VehicleViewModel : IVehicleViewModel
    {
        public string VIN { get; set; }

        public int Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Option { get; set; }

        public int CityMPG { get; set; }

        public int HighwayMPG { get; set; }

        public int CombinedMPG { get; set; }
    }
}