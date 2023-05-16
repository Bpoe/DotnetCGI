namespace Dotnet.Cgi
{
    using System;
    using System.Net;

    public class CgiResult
    {
        public HttpStatusCode StatusCode { get; }

        public object? Value { get; }

        public CgiResult(HttpStatusCode statusCode, object? value)
        {
            StatusCode = statusCode;
            Value = value;
        }

        public async Task ExecuteAsync(CgiContext context)
        {
            context.Response.StatusCode = this.StatusCode;

            if (this.Value != null)
            {
                context.Response.Content = new JsonContent(this.Value);
            }

            await WriteToConsoleAsync(context.Response);
        }

        private async Task WriteToConsoleAsync(HttpResponseMessage response)
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
                var content = await response.Content.ReadAsStringAsync();

                // Headers and content are separated by an empty line
                Console.WriteLine();
                Console.WriteLine(content);
            }
        }
    }
}
