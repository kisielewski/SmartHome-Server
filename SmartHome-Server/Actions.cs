
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
            if(val == 0)
            {
                serial.Write("AB" + val);
            }
            home.AddMessage("n", "AL" + val);
        }

        public void SetTemperature(string name, int val)
        {
            if(home.Sensors["PTMIN"] > val)
            {
                home.SetSensor("PTMIN", val);
            }
            if(home.Sensors["PTMAX"] < val)
            {
                home.SetSensor("PTMAX", val);
            }
        }

        public void DetectAlarm(string name, int val)
        {
            if(home.Controls["AL"] == 1 && val == 1)
            {
                if(home.Controls["AB"] == 0)
                {
                    home.SetControl("AB", 1);
                    home.AddMessage("p", "AB1");
                }
            }
            else if(home.Controls["AB"] == 1 && val == 0)
            {
                home.SetControl("AB", 0);
            }
            DetectLight(name, val);
        }

        public void DetectDoor(string name, int val)
        {
            if(val < 20 && home.Controls["S2"] == 0)
            {
                home.SetControl("S2", 1);
            }
            else if(val > 25 && home.Controls["S2"] == 1)
            {
                home.SetControl("S2", 0);
            }
        }

        public void DetectLight(string name, int val)
        {
            if(home.Controls["L3"] != val)
            {
                home.SetControl("L3", val);
            }
        }
    }
}
