namespace Http_Server;

internal class HttpGET : Attribute
{
    public string MethodURI;

    public HttpGET(string methodUri)
    {
        MethodURI = methodUri;
    }
} 