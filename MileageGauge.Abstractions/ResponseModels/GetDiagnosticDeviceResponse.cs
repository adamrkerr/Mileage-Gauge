using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MileageGauge.Abstractions.ResponseModels
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