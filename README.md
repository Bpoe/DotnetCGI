# .Net CGI
This code illustrates how to implement the Common Gateway Interface standard with C#. I created a simple CgiContext class that will build an
HttpRequestMessage from the environment variables passed by the web server. You can then use that to read the input and respond appropriately.

Why? Why not? This was mostly just educational to see how CGI works as an IPC method.

Sample
```dotnetcli
using Dotnet.Cgi;
using Newtonsoft.Json.Linq;

var app = new CgiApp();

app.Map(HttpMethod.Get, "/cgi/SampleCgiApp.exe/", async (context, parameters) =>
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
```
