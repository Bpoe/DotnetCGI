using System.Net;
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
context.Response.Headers.Add("foo", "bar");

context.Response.WriteToConsole();