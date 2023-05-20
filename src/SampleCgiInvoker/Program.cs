using Dotnet.Cgi;

// Make request
var handler = new CgiHttpMessageHandler("C:\\git\\dotnetCGI\\src\\SampleCgiApp\\bin\\Debug\\net7.0\\SampleCgiApp.exe");
var client = new HttpClient(handler);

var response = await client.GetAsync("http://localhost/");

// Write response to Console
Console.WriteLine
    ($"HTTP/{response.Version.ToString(2)} {(int)response.StatusCode} {ReasonPhrases.GetReasonPhrase((int)response.StatusCode)}");

foreach (var h in response.Headers)
{
    Console.WriteLine($"{h.Key}: {string.Concat(h.Value)}");
}

foreach (var h in response.Content.Headers)
{
    Console.WriteLine($"{h.Key}: {string.Concat(h.Value)}");
}

Console.WriteLine();
Console.WriteLine(await response.Content.ReadAsStringAsync());
