namespace Dotnet.Cgi
{
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public class JsonContent : StringContent
    {
        /// <summary>The media type to use when none is specified.</summary>
        private const string DefaultMediaType = "application/json";

        public JsonContent(object content)
            : base(JsonConvert.SerializeObject(content), new MediaTypeHeaderValue(DefaultMediaType))
        {
            _ = this.TryComputeLength(out var contentLength);
            this.Headers.ContentLength = contentLength;
        }
    }
}
