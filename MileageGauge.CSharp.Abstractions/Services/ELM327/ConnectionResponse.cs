using MileageGauge.CSharp.Abstractions.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ELM327
{
    public class ConnectionResponse : IResponseModel
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
