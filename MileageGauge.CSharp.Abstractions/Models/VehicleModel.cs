using MileageGauge.CSharp.Abstractions.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ViewModels
{
    public class VehicleModel
    {
        public string VIN { get; set; }

        public int Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Option { get; set; }

        public double CityMPG { get; set; }

        public double HighwayMPG { get; set; }

        public double CombinedMPG { get; set; }

        public DateTime LastSelected { get; set; }

        public string Description => $"{Year} {Make} {Model}";
        
    }
}