using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Http_Server
{
    public enum ServerStatus{Start,Stop};
    public class ServerSetting
    {
        public int Port { get; set; } = 7700;
        //public string Path { get; set; } = @"./site";
        public string Path { get; set; } = Directory.GetCurrentDirectory();
    }
    public class HttpServer:IDisposable
    {
        public ServerStatus Status = ServerStatus.Stop;
        private ServerSetting _serverSetting=new ServerSetting();
        private readonly HttpListener _httpListener;
        public HttpServer() 
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:"+ _serverSetting.Port + "/");
        }
        public void Start() 
        { 
             if(Status==ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            Console.WriteLine("Запуск сервера...");
            _httpListener.Start();
            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Start;
            Listening();
        }
        public void Stop() 
        {
            if (Status == ServerStatus.Stop) return;
            Console.WriteLine("Остановка сервера...");
            _httpListener.Stop();
            Status = ServerStatus.Stop;
            Console.WriteLine("Сервер остановлен");
        }
        private void Listening()
        {
            _httpListener.BeginGetContext(new AsyncCallback(ListenerCallBack), _httpListener);
        }
        private void ListenerCallBack(IAsyncResult result) 
        {
            if(_httpListener.IsListening)
            {
                var _httpContext = _httpListener.EndGetContext(result);
                HttpListenerRequest request = _httpContext.Request;
                HttpListenerResponse response = _httpContext.Response;

                byte[] buffer;
                if(File.Exists(_serverSetting.Path + "\\google\\index.html"))
                {
                    buffer = getFile(_httpContext.Request.RawUrl.Replace("%20", " "));
                    
                    if (buffer==null)
                    {
                        response.Headers.Set("Content-Type", "text/plain");
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string err = "404 - not found";
                        buffer = Encoding.UTF8.GetBytes(err);
                    }
                }
                else
                {
                    string err = $" File not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
                Stream output = response.OutputStream;
                output.Write(buffer,0,buffer.Length);
                output.Close();
                Listening();
            }
        }
        private byte[] getFile(string rawUrl) 
        {
            byte[] buffer = null;
            var filePath = _serverSetting.Path + "\\google\\index.html" + rawUrl;
            Console.WriteLine(filePath);
            if (Directory.Exists(filePath))
            {
                Console.WriteLine('5');
                if (File.Exists(filePath))
                {
                    Console.WriteLine('6');
                    buffer = File.ReadAllBytes(filePath);
                }

            }
            else if (File.Exists(filePath))
                Console.WriteLine("7");
                buffer = File.ReadAllBytes(filePath);
            return buffer; 
        }
        public void Dispose() 
        {
            Stop();
        }
    }
}