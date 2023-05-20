namespace Dotnet.Cgi;

public class CgiApp
{
    public List<CgiRoute> Routes { get; } = new List<CgiRoute>();

    public Uri Uri { get; set; } = new Uri("http://localhost");

    public void Map(HttpMethod method, string path, Action<CgiContext, IDictionary<string, string>> handler)
        => this.Routes.Add(new CgiRoute(method, path, handler));

    public async Task Execute()
    {
        var context = CgiContext.GetInstance();

        var (handler, parameters) = this.TryGetHandler(context.Request);
        if (handler == null)
        {
            await context.NotFound();
        }
        else
        {
            handler(context, parameters);
        }

        await WriteToConsoleAsync(context.Response);
    }

    public (Action<CgiContext, IDictionary<string, string>>?, IDictionary<string, string>) TryGetHandler(HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.RequestUri);

        var requestSegments = request.RequestUri.Segments;
        
        foreach (var route in this.Routes)
        {
            if (request.Method != route.Method)
            {
                continue;
            }

            var routeSegments = new Uri(this.Uri, route.Path).Segments;

            var isMatch = IsMatch(routeSegments, requestSegments);

            if (isMatch)
            {
                var variables = GetVariables(routeSegments, requestSegments);
                return (route.Handler, variables);
            }
        }

        return (null, new Dictionary<string, string>());
    }

    public static bool IsMatch(string[] template, string[] request) 
    {
        if (template.Length != request.Length)
        {
            return false;
        }

        for (var i  = 0; i < request.Length; i++)
        {
            if (!string.Equals(template[i], request[i], StringComparison.InvariantCultureIgnoreCase) && !IsVariable(template[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsVariable(string segment)
    {
        var s = Uri.UnescapeDataString(segment).TrimEnd('/');
        return s.StartsWith('{') && s.EndsWith("}");
    }

    public static IDictionary<string, string> GetVariables(string[] template, string[] request)
    {
        var variables = new Dictionary<string, string>();

        if (template.Length != request.Length)
        {
            return variables;
        }

        for (var i = 0; i < request.Length; i++)
        {
            if (IsVariable(template[i]))
            {
                var name = Uri.UnescapeDataString(template[i]).TrimEnd('/').TrimEnd('}').TrimStart('{');
                var value = Uri.UnescapeDataString(request[i]).TrimEnd('/');
                variables.Add(name, value);
            }
        }

        return variables;
    }

    private static async Task WriteToConsoleAsync(HttpResponseMessage response)
    {
        // If "Status: ..." is not returned, then 200 is assumed
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
