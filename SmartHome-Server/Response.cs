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
            foreach(string name in collection)
            {
                home.SetControl(name, int.Parse(collection[name]));
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
    }
}
