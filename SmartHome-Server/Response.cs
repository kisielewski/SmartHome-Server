using System;
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
            return JsonConvert.SerializeObject(home.Controls);
        }

        public void SetControls(NameValueCollection collection)
        {
            foreach (string name in collection)
            {
                if (int.TryParse(collection[name], out int val)){
                    home.SetControl(name, val);
                }
            }
        }

        public string GetMessages()
        {
            return JsonConvert.SerializeObject(home.Messages);
        }

        public string GetSensors()
        {
            return JsonConvert.SerializeObject(home.Sensors);
        }

        public string GetAllContent()
        {
            return JsonConvert.SerializeObject(home);
        }

        public string GetAllContent(DateTime date)
        {
            while (home.LastChange.Equals(date)) ;
            return GetAllContent();
        }
    }
}
