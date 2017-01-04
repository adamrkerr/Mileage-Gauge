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

        double CityMPG { get; set; }

        double HighwayMPG { get; set; }

        double CombinedMPG { get; set; }
    }
}