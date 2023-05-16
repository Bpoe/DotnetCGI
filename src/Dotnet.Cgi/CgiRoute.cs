namespace Dotnet.Cgi
{
    public class CgiRoute
    {
        public CgiRoute(HttpMethod method, string path, Action<CgiContext, IDictionary<string, string>> handler)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            Method = method ?? throw new ArgumentNullException(nameof(method));
            Path = path;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public HttpMethod Method { get; }

        public string Path { get; }

        public Action<CgiContext, IDictionary<string, string>> Handler { get; }
    }
}
