using System.Collections.Generic;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ViewModels
{
    public interface IDiagnosticViewModel
    {
        Task<IEnumerable<string>> GetDiagnosticCodes();

        Task ClearDiagnosticCodes();
    }
}
