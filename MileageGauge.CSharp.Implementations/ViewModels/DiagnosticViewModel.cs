using MileageGauge.CSharp.Abstractions.Services;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.ViewModels
{
    public class DiagnosticViewModel : IDiagnosticViewModel
    {
        private readonly IDiagnosticDeviceService _diagnosticService;

        public DiagnosticViewModel(IDiagnosticDeviceService diagnosticService)
        {
            _diagnosticService = diagnosticService;
        }

        public async Task ClearDiagnosticCodes()
        {
            await _diagnosticService.ClearDiagnosticCodes();
        }

        public async Task<IEnumerable<string>> GetDiagnosticCodes()
        {
            //TODO: response model
            return await _diagnosticService.GetDiagnosticCodes();
        }
    }
}
