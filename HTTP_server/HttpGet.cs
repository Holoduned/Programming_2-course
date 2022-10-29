namespace Homework_01_10_22_week5;

internal class HttpGET : Attribute
{
    public string MethodURI;

    public HttpGET(string methodUri)
    {
        MethodURI = methodUri;
    }
}