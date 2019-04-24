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
            string responseString = "";
            
            switch (request.Url.AbsolutePath)
            {
                case "/controls/get":
                    responseString = controller.GetControls();
                    break;
                case "/controls/set":
                    string query = request.Url.Query;
                    NameValueCollection collection = HttpUtility.ParseQueryString(query);
                    controller.SetControls(collection);
                    break;
                case "/messages/get":
                    responseString = controller.GetMessages();
                    break;
                case "/sensors/get":
                    responseString = controller.GetSensors();
                    break;
                default:
                    response.StatusCode = 404;
                    break;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.AddHeader("Access-Control-Allow-Origin", "*");
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
