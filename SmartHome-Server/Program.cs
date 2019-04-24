
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
            home.DefaultCotrolsAction = new SmartHome.TypeControlsAction(actions.OnlySendControl);
            home.ControlsActions.Add("AL", new SmartHome.TypeControlsAction(actions.SendAlarm));

            HttpServer server = new HttpServer(home);
            server.Start();

            home.AddMessage("p", "http");
        }
    }
}
