namespace Dotnet.Cgi
{
    using System;
    using System.Net;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public static class HttpMessageExtensions
    {
        public static void WriteToConsole(this HttpResponseMessage response)
        {
            // If "Status: ..." is not returned, then 200 us assumed
            Console.WriteLine("Status: {0} {1}", (int)response.StatusCode, ReasonPhrases.GetReasonPhrase((int)response.StatusCode));

            var finalHeaders = new Dictionary<string, string>();

            foreach (var h in response.Headers)
            {
                finalHeaders.Add(h.Key, string.Concat(h.Value));
            }

            if (response.Content != null)
            {
                // NOTE: If content is returned, then the Content-Type header is required!
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

                // Headers and content are separated by an empty line
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
