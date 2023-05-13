﻿using System.Net;
using System.Net.Http.Headers;
using Dotnet.Cgi;
using Newtonsoft.Json.Linq;

Console.WriteLine("\r\n\r\n");

var context = CgiContext.GetInstance();

var responseContent = new JObject
{
    ["env"] = JObject.FromObject(Environment.GetEnvironmentVariables()),
    ["context"] = JObject.FromObject(context),
    ["requestBody"] = context.Request.Content?.ReadAsStringAsync().Result,
};

context.Response.StatusCode = HttpStatusCode.NotFound;
context.Response.Content = new StringContent(responseContent.ToString(), new MediaTypeHeaderValue("application/json"));


Console.WriteLine(context.Response.Content.ReadAsStringAsync().Result);