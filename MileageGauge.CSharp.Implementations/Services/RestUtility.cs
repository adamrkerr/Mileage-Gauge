using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Json;
using System.Net;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.Services
{
    class RestUtility : IRestUtility
    {
        public async Task<JsonValue> ExecuteGetRequestAsync(string url)
        {
            // Create an HTTP web request using the URL:
            var request = HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (var stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    var jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }
    }
}