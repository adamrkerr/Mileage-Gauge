using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MileageGauge.CSharp.Abstractions.ViewModels
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