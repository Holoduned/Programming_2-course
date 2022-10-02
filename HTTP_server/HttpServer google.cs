using System;
using System.Net;
using System.IO;
using System.Runtime.CompilerServices;

namespace NetConsoleApp
{
    class HttpServer
    {
        private HttpListener listener;
        public void Listen()
        {
            string path = Directory.GetCurrentDirectory();
            if (!File.Exists(path + "\\google\\index.html"))
            {
                Console.WriteLine("Обнаружена ошибка");
                Stop();
                Console.WriteLine("Файл html не найден, пожалуйста вложите файл в \\google\\ директорию");

                while (true)
                {
                    Console.WriteLine($"Введите {"Start"} для запуска сервера");
                    var input = Console.ReadLine();
                    if (input == "Start")
                    {
                        if (File.Exists(path + "\\google\\index.html"))
                        { Console.Clear(); Start(); break; }
                        Console.WriteLine("Файл html не найден, пожалуйста вложите файл в \\google\\ директорию");
                    };
                }
                
            }
            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                response.Headers.Set("Content-Type", "text/html");
                byte[] buffer = getFile(context.Request.RawUrl.Replace("%20", " "));
                //response.ContentLength64 = buffer.Length;

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                Console.WriteLine("Обработка подключений завершена");
            }
        }

        public void Start()
        {
            Console.WriteLine("Запуск сервера..");

            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/google/");
            listener.Start();

            Console.WriteLine("Сервер запущен");
            Listen();
        }

        public void Stop()
        {
            listener.Stop();
            Console.WriteLine("Сервер остановлен");
        }

        private byte[] getFile(string rawUrl)
        {
            byte[] buffer = null;
            var pathFile = Directory.GetCurrentDirectory() + rawUrl;

            if (Directory.Exists(pathFile))
            {
                pathFile = pathFile + "/index.html";
                if(File.Exists(pathFile))
                {
                    buffer = File.ReadAllBytes(pathFile);
                }
            }
            else if (File.Exists(pathFile))
            {
                buffer = File.ReadAllBytes(pathFile);
            }

            return buffer;
        }
    }
}