
namespace SmartHome_Server
{
    public class Actions
    {
        private Serial serial;
        private SmartHome home;

        public Actions(Serial serial, SmartHome home)
        {
            this.serial = serial;
            this.home = home;
        }

        public void OnlySendControl(string name, int val)
        {
            serial.Write(name + val);
            home.AddMessage("n", name + val);
        }

        public void SendAlarm(string name, int val)
        {
            serial.Write("AL" + val);
            serial.Write("AB" + val);
            home.AddMessage("n", "AL" + val);
        }
    }
}
