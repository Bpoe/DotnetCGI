namespace Dotnet.Cgi
{
    using System;
    using System.Net.Http.Headers;

    public class CgiContext
    {
        public HttpRequestMessage Request { get; set; }

        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage();

        public static CgiContext GetInstance()
        {
            var requestUri = new UriBuilder()
            {
                Port = int.Parse(Environment.GetEnvironmentVariable(CgiEnvironmentVariable.ServerPort) ?? "0"),
                Path = Environment.GetEnvironmentVariable(CgiEnvironmentVariable.RequestUri)
                    ?? Environment.GetEnvironmentVariable(CgiEnvironmentVariable.ScriptName) + Environment.GetEnvironmentVariable(CgiEnvironmentVariable.PathInfo),
                Query = Environment.GetEnvironmentVariable(CgiEnvironmentVariable.QueryString) ?? string.Empty,
            };

            var context = new CgiContext();

            context.Request = new HttpRequestMessage(
                new HttpMethod(Environment.GetEnvironmentVariable(CgiEnvironmentVariable.RequestMethod) ?? string.Empty),
                requestUri.Uri);

            var httpVersion = Environment.GetEnvironmentVariable(CgiEnvironmentVariable.ServerProtocol) ?? "HTTP/1.1";
            context.Request.Version = new Version(httpVersion.Substring(5));

            GetContent(context.Request);

            LoadHttpHeadersFromEnvironment(context.Request.Headers);

            return context;
        }

        private static void GetContent(HttpRequestMessage request)
        {
            var contentLength = long.Parse(Environment.GetEnvironmentVariable(CgiEnvironmentVariable.ContentLength) ?? "0");

            if (contentLength > 0)
            {
                var content = new byte[contentLength];

                _ = Console.OpenStandardInput().Read(content, 0, (int)contentLength);

                request.Content = new ByteArrayContent(content);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(Environment.GetEnvironmentVariable(CgiEnvironmentVariable.ContentType) ?? string.Empty);
                request.Content.Headers.ContentLength = contentLength;
            }
        }

        private static void LoadHttpHeadersFromEnvironment(HttpHeaders headers)
        {
            var envVariables = Environment.GetEnvironmentVariables();

            var e = envVariables.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    var entry = e.Entry;
                    var key = (string)entry.Key;

                    if (key.StartsWith(CgiEnvironmentVariable.HttpHeadersPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        key = key.Substring(CgiEnvironmentVariable.HttpHeadersPrefix.Length);
                        headers.Add(key, (string?)entry.Value ?? string.Empty);
                    }
                }
            }
            finally
            {
                (e as IDisposable)?.Dispose();
            }
        }
    }
}
