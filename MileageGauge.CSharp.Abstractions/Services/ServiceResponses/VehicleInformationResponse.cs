using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ServiceResponses
{
    public class VehicleInformationResponse
    {
        public string Make { get; set; }
        public string SelectedModel { get; set; }
        public List<string> VehicleModels { get; set; }
        public VehicleInformationResponseOption SelectedVehicleOption { get; set; }
        public List<VehicleInformationResponseOption> VehicleOptions { get; set; }
        public string VIN { get; set; }
        public int Year { get; set; }
    }

    public class VehicleInformationResponseOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
