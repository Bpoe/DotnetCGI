using Dotnet.Cgi;
using Newtonsoft.Json.Linq;

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