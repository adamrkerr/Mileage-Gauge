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
        public async Task<T> ExecuteGetRequestAsync<T>(string url) where T : new()
        {
            // Create an HTTP web request using the URL:
            var request =(HttpWebRequest) HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            var serializer = new JsonSerializer();

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            // Get a stream representation of the HTTP web response:
            using (var stream = response.GetResponseStream())
            // get the stream reader
            using (var sr = new StreamReader(stream))
            {
                var stringObject = await sr.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(stringObject))
                {
                    return new T();
                }
                else if (stringObject.Equals("null", StringComparison.InvariantCultureIgnoreCase)) // there are actually services that do this :(
                {
                    return new T();
                }
                // Use this stream to build a JSON document object:
                var jsonDoc = JsonConvert.DeserializeObject<T>(stringObject);

                // Return the JSON document:
                return jsonDoc;
            }

        }
    }
}