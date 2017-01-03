using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Json;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IRestUtility
    {
        Task<JsonValue> ExecuteGetRequestAsync(string url);
    }
}