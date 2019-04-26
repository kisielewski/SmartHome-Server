using System;
using System.Threading;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace SmartHome_Server
{
    public class Response
    {
        private SmartHome home;

        public Response(SmartHome home)
        {
            this.home = home;
        }

        public string GetControls()
        {
            home.Semaphore.WaitOne();
            string result = JsonConvert.SerializeObject(home.Controls);
            home.Semaphore.Release();
            return result;
        }

        public void SetControls(NameValueCollection collection)
        {
            foreach (string name in collection)
            {
                if (int.TryParse(collection[name], out int val)){
                    home.Semaphore.WaitOne();
                    home.SetControl(name, val);
                    home.Semaphore.Release();
                }
            }
        }

        public string GetMessages()
        {
            home.Semaphore.WaitOne();
            string result = JsonConvert.SerializeObject(home.Messages);
            home.Semaphore.Release();
            return result;
        }

        public string GetSensors()
        {
            home.Semaphore.WaitOne();
            string result = JsonConvert.SerializeObject(home.Sensors);
            home.Semaphore.Release();
            return result;
        }

        public string GetAllContent()
        {
            home.Semaphore.WaitOne();
            string result = JsonConvert.SerializeObject(home);
            home.Semaphore.Release();
            return result;
        }

        public string GetAllContent(DateTime date)
        {
            while (home.LastChange.Equals(date))
            {
                Thread.Sleep(100);
            }
            return GetAllContent();
        }
    }
}
