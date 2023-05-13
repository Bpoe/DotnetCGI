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

Console.WriteLine("Status: {0} {1}", (int)context.Response.StatusCode, ReasonPhrases.GetReasonPhrase((int)context.Response.StatusCode));

foreach (var h in context.Response.Headers)
{
    Console.WriteLine("{0}: {1}", h.Key, string.Concat(h.Value));
}

foreach (var h in context.Response.Content.Headers)
{
    Console.WriteLine("{0}: {1}", h.Key, string.Concat(h.Value));
}

Console.WriteLine();
Console.WriteLine(context.Response.Content.ReadAsStringAsync().Result);
