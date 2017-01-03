using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MileageGauge.CSharp.Abstractions.ResponseModels
{
    public interface IResponseModel
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}