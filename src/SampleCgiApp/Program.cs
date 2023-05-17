using Dotnet.Cgi;
using Newtonsoft.Json.Linq;

if (args.Length > 1)
{
    Environment.SetEnvironmentVariable(CgiEnvironmentVariable.RequestMethod, args[0]);
    Environment.SetEnvironmentVariable(CgiEnvironmentVariable.RequestUri, args[1]);
}

var app = new CgiApp();

app.Map(HttpMethod.Get, "/", async (context, parameters) =>
{
    var responseContent = new JObject
    {
        ["env"] = JObject.FromObject(Environment.GetEnvironmentVariables()),
        ["context"] = JObject.FromObject(context),
        ["requestBody"] = context.Request.Content?.ReadAsStringAsync().Result,
    };

    await context.Created(responseContent);
});

await app.Execute();