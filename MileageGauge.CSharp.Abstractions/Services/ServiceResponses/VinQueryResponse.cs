using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ServiceResponses
{
    public class VinQueryResponse
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public string SearchCriteria { get; set; }
        public List<VinQueryResponseItem> Results { get; set; }
    }

    public class VinQueryResponseItem
    {
        public string Value { get; set; }
        public string ValueId { get; set; }
        public string Variable { get; set; }
        public int VariableId { get; set; }
    }
}
