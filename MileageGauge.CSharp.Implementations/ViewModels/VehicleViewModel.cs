using MileageGauge.CSharp.Abstractions.ViewModels;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class VehicleViewModel : IVehicleViewModel
    {
        public string VIN { get; set; }

        public int Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Option { get; set; }

        public double CityMPG { get; set; }

        public double HighwayMPG { get; set; }

        public double CombinedMPG { get; set; }
    }
}