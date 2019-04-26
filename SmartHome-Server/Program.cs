
namespace SmartHome_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Equals("devel"))
            {
                Global.Devel = true;
            }
            SmartHome home = new SmartHome();
            Serial serial = new Serial(args[0], home);
            Actions actions = new Actions(serial, home);

            //bind actions
            home.DefaultCotrolsAction = new SmartHome.TypeControlsAction(actions.SendControl);
            home.ControlsActions.Add("AL", new SmartHome.TypeControlsAction(actions.SendAlarm));
            home.ControlsActions.Add("APR", new SmartHome.TypeControlsAction(actions.OnlyAddMessage));
            home.ControlsActions.Add("APO", new SmartHome.TypeControlsAction(actions.OnlyAddMessage));
            home.SensorsActions.Add("PT", new SmartHome.TypeSensorsAction(actions.SetTemperature));
            home.SensorsActions.Add("PR", new SmartHome.TypeSensorsAction(actions.DetectAlarm));
            home.SensorsActions.Add("PO", new SmartHome.TypeSensorsAction(actions.DetectDoor));

            HttpServer server = new HttpServer(args[1], home);
            server.Start();

            home.AddMessage("p", "http");
        }
    }
}
