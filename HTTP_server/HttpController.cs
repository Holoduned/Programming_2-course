namespace Homework_01_10_22_week5;

internal class HttpController : Attribute
{
    public string ControllerName;

    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}