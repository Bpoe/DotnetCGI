# .Net CGI
This code illustrates how to implement the Common Gateway Interface standard with C#. I created a simple CgiContext class that will build an
HttpRequestMessage from the environment variables passed by the web server. You can then use that to read the input and respond appropriately.

Why? Why not? This was mostly just educational to see how CGI works as an IPC method.

Sample
```dotnetcli
using System.Net;
using System.Net.Http.Headers;
using Dotnet.Cgi;
using Newtonsoft.Json.Linq;

var context = CgiContext.GetInstance();

var responseContent = new JObject
{
    ["env"] = JObject.FromObject(Environment.GetEnvironmentVariables()),
    ["context"] = JObject.FromObject(context),
    ["requestBody"] = context.Request.Content?.ReadAsStringAsync().Result,
};

context.Response.StatusCodeResult(HttpStatusCode.BadRequest, responseContent);
context.Response.Headers.Add("X-foo", "bar");

context.Response.WriteToConsole();
```
