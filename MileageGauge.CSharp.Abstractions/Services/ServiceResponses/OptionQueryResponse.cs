using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ServiceResponses
{
    public class OptionQueryResponse
    {
        public List<OptionQueryResponseItem> MenuItem { get; set; }
    }

    public class OptionQueryResponseItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
