namespace Dotnet.Cgi
{
    using System.Net;

    public static class CgiContextExtensions
    {
        public static Task Created(this CgiContext context, object? value = null)
            => new CgiResult(HttpStatusCode.Created, value).ExecuteAsync(context);

        public static Task Ok(this CgiContext context, object? value = null)
            => new CgiResult(HttpStatusCode.OK, value).ExecuteAsync(context);

        public static Task BadRequest(this CgiContext context, object? value = null)
            => new CgiResult(HttpStatusCode.BadRequest, value).ExecuteAsync(context);

        public static Task NotFound(this CgiContext context, object? value = null)
            => new CgiResult(HttpStatusCode.NotFound, value).ExecuteAsync(context);

    }
}
