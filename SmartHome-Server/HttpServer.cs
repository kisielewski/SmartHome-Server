using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Web;

namespace SmartHome_Server
{
    public class HttpServer
    {
        private SmartHome home;
        private Thread HttpThread;

        public HttpServer(SmartHome home)
        {
            this.home = home;
        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener is not supported");
                return;
            }
            HttpThread = new Thread(Loop);
            HttpThread.Start();
        }

        private void Loop()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:80/");
            listener.Start();
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                Thread t = new Thread(new ParameterizedThreadStart(NewRequest));
                t.Start(context);
            }
        }

        private void NewRequest(object _context)
        {
            HttpListenerContext context = (HttpListenerContext)_context;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Response controller = new Response(home);
            NameValueCollection collection;
            string query = request.Url.Query;
            string responseString = "";
            byte[] responseBytes = null;
            string path = "files"+request.Url.AbsolutePath;
            if(request.Url.AbsolutePath == "/")
            {
                path += "index.html";
            }
            
            switch (request.Url.AbsolutePath)
            {
                case "/api/content/get":
                    collection = HttpUtility.ParseQueryString(query);
                    if(collection["lastchange"] != null)
                    {
                        string dateString = collection["lastchange"].Replace(' ', '+').Replace("\"", "");
                        if (DateTime.TryParse(dateString, out DateTime date))
                        {
                            responseString = controller.GetAllContent(date);
                            break;
                        }
                    }
                    responseString = controller.GetAllContent();
                    break;
                case "/api/controls/set":
                    collection = HttpUtility.ParseQueryString(query);
                    controller.SetControls(collection);
                    break;
                case "/api/controls/get":
                    responseString = controller.GetControls();
                    break;
                case "/api/messages/get":
                    responseString = controller.GetMessages();
                    break;
                case "/api/sensors/get":
                    responseString = controller.GetSensors();
                    break;
                default:
                    if (File.Exists(path))
                    {
                        responseBytes = File.ReadAllBytes(path);
                        response.ContentType = MimeMapping.GetMimeMapping(path);
                    }
                    else
                    {
                        response.StatusCode = 404;
                    }
                    break;
            }
            byte[] buffer = null;
            Stream output = response.OutputStream;
            if (responseBytes != null && responseBytes.Length > 0)
            {
                response.ContentLength64 = responseBytes.Length;
                output.Write(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.AddHeader("Access-Control-Allow-Origin", "*");
                output.Write(buffer, 0, buffer.Length);
            }
           
            output.Close();
        }
    }
}
