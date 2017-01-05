using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public class LoadVehicleDetailsModelRequiredResponse : LoadVehicleDetailsResponse
    {
        public List<string> ModelOptions { get; set; }
    }
}
