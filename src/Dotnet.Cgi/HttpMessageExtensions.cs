namespace Dotnet.Cgi
{
    using System;

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
                Console.WriteLine("Content-Length: {0}", content.Length);
                Console.WriteLine();
                Console.WriteLine(content);
            }
        }
    }
}
