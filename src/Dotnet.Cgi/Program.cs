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

context.Response.StatusCode = HttpStatusCode.BadRequest;
context.Response.Content = new StringContent(responseContent.ToString(), new MediaTypeHeaderValue("application/json"));
context.Response.Headers.Add("foo", "bar");

context.Response.WriteToConsole();