using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Http_Server
{
    enum ServerStatus{Start,Stop};
    public class ServerSetting
    {
        public int Port { get; set; } = 7700;
        public string Path { get; set; } = @"./site";
    }
    internal class HttpServer:IDisposable
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
        private async void Listening()
        {
            while (_httpListener.IsListening){
                
                var _httpContext = await _httpListener.GetContextAsync();

                if (MethodHandler(_httpContext)) return;

                StaticFiles(_httpContext.Request, _httpContext.Response);
               
            }
            //_httpListener.BeginGetContext(new AsyncCallback(ListenerCallBack), _httpListener);
        }

        private void StaticFiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            byte[] buffer;
            if (Directory.Exists(_serverSetting.Path))
            {
                buffer = getFile(request.RawUrl.Replace("%20", " "));


                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                    string err = "not found - 404";
                    buffer = Encoding.UTF8.GetBytes(err);
                }

            }
            else
            {
                string err = $"Directory '{_serverSetting.Path}' not found";
                buffer = Encoding.UTF8.GetBytes(err);
            }

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

/*        {
            if(_httpListener.IsListening)
            {
                var _httpContext = _httpListener.EndGetContext(result);
                HttpListenerRequest request = _httpContext.Request;
                HttpListenerResponse response = _httpContext.Response;
                byte[] buffer;
                if(Directory.Exists(_serverSetting.Path))
                {
                    buffer = FileManager.getFile(_httpContext.Request.RawUrl.Replace("%20", " "), _serverSetting);
                    
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
                    string err = $"Directory "+ _serverSetting.Path + " not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
                Stream output = response.OutputStream;
                output.Write(buffer,0,buffer.Length);
                output.Close();
                Listening();
            }
        }*/
        public void Dispose() 
        {
            Stop();
        }
        
        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                                                 .FirstOrDefault();
            
            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
            
            return true;
        }
        private byte[]? getFile(string rawUrl)
        {
            byte[]? buffer = null;
            var filePath = _serverSetting.Path + rawUrl.Replace('/','\\');

            if (Directory.Exists(filePath))
            {

                filePath = filePath + "index.html";

                if (File.Exists(filePath))
                {
                    //Console.WriteLine(filePath);
                    buffer = File.ReadAllBytes(filePath);
                }

            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
          
            return buffer;
        }
    }
}