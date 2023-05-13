namespace Dotnet.Cgi
{
    public static class CgiEnvironmentVariable
    {
        public const string HttpHeadersPrefix = "HTTP_";
        public const string RequestMethod = "REQUEST_METHOD";
        public const string RequestUri = "REQUEST_URI";
        public const string GatewayInterface = "GATEWAY_INTERFACE";
        public const string ServerName = "SERVER_NAME";
        public const string ServerSoftware = "SERVER_SOFTWARE";
        public const string ServerProtocol = "SERVER_PROTOCOL";
        public const string ServerPort = "SERVER_PORT";
        public const string PathInfo = "PATH_INFO";
        public const string PathTranslated = "PATH_TRANSLATED";
        public const string ScriptName = "SCRIPT_NAME";
        public const string DocumentRoot = "DOCUMENT_ROOT";
        public const string QueryString = "QUERY_STRING";
        public const string RemoteAddress = "REMOTE_ADDR";
        public const string RemoteHost = "REMOTE_HOST";
        public const string AuthenticationType = "AUTH_TYPE";
        public const string RemoteUser = "REMOTE_USER";
        public const string RemoteIdentity = "REMOTE_IDENT";
        public const string ContentType = "CONTENT_TYPE";
        public const string ContentLength = "CONTENT_LENGTH";
    }
}
