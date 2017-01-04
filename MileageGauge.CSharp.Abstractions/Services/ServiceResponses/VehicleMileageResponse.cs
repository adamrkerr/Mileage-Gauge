using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ServiceResponses
{
    public class VehicleMileageResponse
    {
        public double CityMpg { get; set; }
        public double CombinedMpg { get; set; }
        public double HighwayMpg { get; set; }
    }
}
