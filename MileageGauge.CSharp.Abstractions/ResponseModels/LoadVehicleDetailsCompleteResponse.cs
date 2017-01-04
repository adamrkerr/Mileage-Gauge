using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public class LoadVehicleDetailsCompleteResponse : LoadVehicleDetailsResponse
    {
        public bool DetailsAreFromStorage { get; set; }
    }
}
