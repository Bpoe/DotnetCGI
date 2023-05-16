namespace Dotnet.Cgi
{
    using System.Net;

    public class CgiResult
    {
        public HttpStatusCode StatusCode { get; }

        public object? Value { get; }

        public CgiResult(HttpStatusCode statusCode, object? value)
        {
            StatusCode = statusCode;
            Value = value;
        }

        public Task ExecuteAsync(CgiContext context)
        {
            context.Response.StatusCode = this.StatusCode;

            if (this.Value != null)
            {
                context.Response.Content = new JsonContent(this.Value);
            }

            return Task.CompletedTask;
        }
    }
}
