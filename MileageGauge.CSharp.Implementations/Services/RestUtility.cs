using MileageGauge.CSharp.Abstractions.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MileageGauge.CSharp.Implementations.Services
{
    public class RestUtility : IRestUtility
    {
        public async Task<T> ExecuteGetRequestAsync<T>(string url)
        {
            // Create an HTTP web request using the URL:
            var request = HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            var serializer = new JsonSerializer();

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            // Get a stream representation of the HTTP web response:
            using (var stream = response.GetResponseStream())
            // get the stream reader
            using (var sr = new StreamReader(stream))
            //now a json reader
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                // Use this stream to build a JSON document object:
                var jsonDoc = await Task.Run(() => serializer.Deserialize<T>(jsonTextReader));

                // Return the JSON document:
                return jsonDoc;
            }

        }
    }
}