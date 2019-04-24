using System;
using System.IO.Ports;
using System.Threading;

namespace SmartHome_Server
{
    public class Serial
    {
        private SerialPort serialPort;
        private SmartHome home;
        private Thread readThread;

        public Serial(string portName, SmartHome home)
        {
            this.home = home;
            if (!Global.Devel)
            {
                serialPort = new SerialPort();
                serialPort.PortName = portName;
                serialPort.BaudRate = 9600;
                serialPort.Open();
            }
            readThread = new Thread(Read);
            readThread.Start();
        }

        public void Write(string data)
        {
            if (!Global.Devel)
                serialPort.WriteLine(data);
            else
                Console.WriteLine(data);
        }

        private void Read()
        {
            string data;
            while (true)
            {
                try
                {
                    if (!Global.Devel)
                        data = serialPort.ReadLine();
                    else
                        data = Console.ReadLine();
                }
                catch (TimeoutException)
                {
                    continue;
                }
                if (data.Equals("D"))
                {
                    home.AddMessage("p", "D");
                    continue;
                }
                string name = data.Substring(0, 2);
                int val = int.Parse(data.Substring(2, data.Length-2));
                home.SetSensor(name, val);
            }
        }
    }
}
