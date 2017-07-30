using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public class AddVehicleToCollectionResponse : IResponseModel
    {
        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
