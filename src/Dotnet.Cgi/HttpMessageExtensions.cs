namespace Dotnet.Cgi
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.Design.Serialization;
    using System.Net;
    using System.Net.Http.Headers;

    public static class HttpMessageExtensions
    {
        public static void WriteToConsole(this HttpResponseMessage response)
        {
            Console.WriteLine("Status: {0} {1}", (int)response.StatusCode, ReasonPhrases.GetReasonPhrase((int)response.StatusCode));

            var finalHeaders = new Dictionary<string, string>();

            foreach (var h in response.Headers)
            {
                finalHeaders.Add(h.Key, string.Concat(h.Value));
            }

            if (response.Content != null)
            {
                foreach (var h in response.Content.Headers)
                {
                    _ = finalHeaders.TryAdd(h.Key, string.Concat(h.Value));
                }
            }

            foreach (var h in finalHeaders)
            {
                Console.WriteLine("{0}: {1}", h.Key, h.Value);
            }

            if (response.Content != null)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine();
                Console.WriteLine(content);
            }
        }

        public static void StatusCodeResult(this HttpResponseMessage response, HttpStatusCode statusCode, object content)
        {
            var contentString = JsonConvert.SerializeObject(content);
            response.StatusCode = statusCode;
            response.Content = new StringContent(
                contentString,
                new MediaTypeHeaderValue("application/json"));

            response.Content.Headers.ContentLength = contentString.Length;
        }
    }
}
