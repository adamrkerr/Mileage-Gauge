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

namespace MileageGauge.Abstractions
{
    public interface IVehicleViewModel
    {
        string VIN { get; set; }

        int Year { get; set; }

        string Make { get; set; }

        string Model { get; set; }

        string Option { get; set; }

        int CityMPG { get; set; }

        int HighwayMPG { get; set; }

        int CombinedMPG { get; set; }
    }
}