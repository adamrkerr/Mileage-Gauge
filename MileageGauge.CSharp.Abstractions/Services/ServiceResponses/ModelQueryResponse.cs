using MileageGauge.CSharp.Abstractions.Services.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Services.ServiceResponses
{
    public class ModelQueryResponse
    {

        [JsonConverter(typeof(SingleOrCollectionConverter<ModelQueryResponseItem>))]
        public List<ModelQueryResponseItem> MenuItem { get; set; }
    }

    public class ModelQueryResponseItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
