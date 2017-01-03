using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public class GetDiagnosticDeviceResponse : IResponseModel
    {
        public string Message
        {
            get; set;
        }

        public bool Success
        {
            get; set;
        }
    }
}