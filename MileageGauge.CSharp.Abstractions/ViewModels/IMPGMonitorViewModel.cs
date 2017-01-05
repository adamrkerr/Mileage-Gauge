using MileageGauge.CSharp.Abstractions.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ViewModels
{
    public interface IMPGMonitorViewModel
    {
        Task BeginMonitoringMPG();

        Task EndMonitoringMPG();

        Action<MPGUpdateResponse> UpdateMPG { get; set; }
    }
}
