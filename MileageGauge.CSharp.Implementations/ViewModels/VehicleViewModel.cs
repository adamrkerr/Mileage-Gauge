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

        public int CityMPG { get; set; }

        public int HighwayMPG { get; set; }

        public int CombinedMPG { get; set; }
    }
}