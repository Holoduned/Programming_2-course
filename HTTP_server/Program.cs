using NetConsoleApp;

namespace NetConsoleApp
{
    public static class Programm
    {
        static void Main(string[] args)
        {
            var server = new HttpServer();
            server.Start();
        }
    }
}
