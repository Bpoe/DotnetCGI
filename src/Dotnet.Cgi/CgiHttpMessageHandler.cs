namespace Dotnet.Cgi
{
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading;

    public class CgiHttpMessageHandler : HttpMessageHandler
    {
        private readonly string pathToExecutable;

        public CgiHttpMessageHandler(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            this.pathToExecutable = path;
        }

        public static bool IsCgiUri(Uri uri)
            => string.Equals("file", uri.Scheme, StringComparison.InvariantCultureIgnoreCase)
                && uri.Segments.Any(
                    s => s.TrimEnd('/').EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase));

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
            => this.SendAsync(request, cancellationToken).Result;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startInfo = ToProcessStartInfo(request, this.pathToExecutable);

            var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start CGI process.");

            if (request.Content != null)
            {
                await request.Content.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
            }

            var response = await ToResponseMessage(process, request);

            await process.WaitForExitAsync(cancellationToken);

            return response;
        }

        private static async Task<HttpResponseMessage> ToResponseMessage(Process process, HttpRequestMessage request)
        {
            var response = new HttpResponseMessage
            {
                RequestMessage = request,
            };

            var headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                var keyValue = line.Split(':', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);
                if (keyValue.Length != 2)
                {
                    throw new InvalidOperationException();
                }

                if (string.Equals("status", keyValue[0], StringComparison.InvariantCultureIgnoreCase))
                {
                    var code = keyValue[1].Trim().Split(' ')[0];
                    response.StatusCode = (HttpStatusCode)int.Parse(code);
                    continue;
                }

                headers.Add(keyValue[0], keyValue[1].Trim());
            }

            if (!process.StandardOutput.EndOfStream)
            {
                if (headers.ContainsKey("content-type"))
                {
                    response.Content = new StringContent(
                        await process.StandardOutput.ReadToEndAsync(),
                        new MediaTypeHeaderValue(headers["content-type"]));
                }
                else
                {
                    response.Content = new StringContent(
                        await process.StandardOutput.ReadToEndAsync());
                }
            }

            foreach (var h in headers)
            {
                if (!h.Key.StartsWith("content-", StringComparison.InvariantCultureIgnoreCase))
                {
                    response.Headers.Add(h.Key, h.Value);
                }
            }

            return response;
        }

        private static ProcessStartInfo ToProcessStartInfo(HttpRequestMessage request, string pathToExecutable)
        {
            var startInfo = new ProcessStartInfo(pathToExecutable, $"{request.Method} {request.RequestUri?.PathAndQuery}");
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.ServerPort] = request.RequestUri?.Port.ToString();
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.QueryString] = request.RequestUri?.Query;
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.RequestMethod] = request.Method.ToString();
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.RequestUri] = request.RequestUri?.PathAndQuery;
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.ContentLength] = request.Content?.Headers.ContentLength?.ToString();
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.ContentType] = request.Content?.Headers.ContentType?.MediaType;
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.ServerProtocol] = "HTTP/" + request.Version.ToString(2);
            startInfo.EnvironmentVariables[CgiEnvironmentVariable.PathInfo] = request.RequestUri?.PathAndQuery;

            foreach (var header in request.Headers)
            {
                startInfo.EnvironmentVariables[CgiEnvironmentVariable.HttpHeadersPrefix + header.Key.Replace('-', '_')] = string.Join(",", header.Value);
            }

            return startInfo;
        }
    }
}
