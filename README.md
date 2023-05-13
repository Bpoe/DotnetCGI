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

Console.WriteLine("\r\n\r\n");

var context = CgiContext.GetInstance();

var responseContent = new JObject
{
    ["color"] = JObject.FromObject(Environment.GetEnvironmentVariables()),
    ["context"] = JObject.FromObject(context),
    ["requestBody"] = context.Request.Content?.ReadAsStringAsync().Result,
};

context.Response.StatusCode = HttpStatusCode.NotFound;
context.Response.Content = new StringContent(responseContent.ToString(), new MediaTypeHeaderValue("application/json"));

Console.WriteLine("Status: {0} {1}", (int)context.Response.StatusCode, ReasonPhrases.GetReasonPhrase((int)context.Response.StatusCode));
Console.WriteLine();
Console.WriteLine(context.Response.Content.ReadAsStringAsync().Result);
```
