using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services
{
    public interface IRestUtility
    {
        Task<T> ExecuteGetRequestAsync<T>(string url);
    }
}