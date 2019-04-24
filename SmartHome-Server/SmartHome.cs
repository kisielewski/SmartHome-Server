using System;
using System.Collections.Generic;

namespace SmartHome_Server
{
    public class SmartHome
    {
        public Dictionary<string, int> Controls { get; private set; }
        public Dictionary<string, string> Sensors { get; private set; }
        public Queue<Message> Messages { get; private set; }
        
        public delegate void TypeControlsAction(string name, int val);
        public TypeControlsAction DefaultCotrolsAction;
        public Dictionary<string, TypeControlsAction> ControlsActions;
        public delegate void TypeSensorsAction(string name, string val);
        public TypeSensorsAction DefaultSensorAction;
        public Dictionary<string, TypeSensorsAction> SensorsActions;

        public SmartHome()
        {
            Controls = new Dictionary<string, int>
            {
                { "L1", 0 },
                { "L2", 0 },
                { "L3", 0 },
                { "L4", 0 },
                { "L5", 0 },
                { "L6", 0 },
                { "R", 0 },
                { "S1", 0 },
                { "S2", 0 },
                { "S3", 0 },
                { "CR", 0 },
                { "CO", 0 },
                { "CS", 0 },
                { "CG", 0 },
                { "CT", 0 },
                { "AL", 0 },
                { "AB", 0 }
            };
            Sensors = new Dictionary<string, string>
            {
                { "PT", "X" },
                { "PW", "X" },
                { "PC", "X" },
                { "PR", "X" },
                { "PO", "X" },
                { "PTMIN", "X" },
                { "PTMAX", "X" }
            };
            Messages = new Queue<Message>();

            ControlsActions = new Dictionary<string, TypeControlsAction>();
            SensorsActions = new Dictionary<string, TypeSensorsAction>();
        }

        public void SetControl(string name, int val)
        {
            if (!Controls.ContainsKey(name))
            {
                return;
            }
            if(val != 0 && val != 1)
            {
                return;
            }
            Controls[name] = val;

            if (ControlsActions.ContainsKey(name))
            {
                ControlsActions[name](name, val);
            }
            else
            {
                DefaultCotrolsAction?.Invoke(name, val);
            }
        }

        public void SetSensor(string name, string val)
        {
            if (!Sensors.ContainsKey(name))
            {
                return;
            }
            Sensors[name] = val;

            if (SensorsActions.ContainsKey(name))
            {
                SensorsActions[name](name, val);
            }
            else
            {
                DefaultSensorAction?.Invoke(name, val);
            }
        }

        public void AddMessage(string type, string content)
        {
            Message message = new Message
            {
                Date = DateTime.Now.ToString(),
                Type = type,
                Content = content
            };
            if(Messages.Count >= 20)
            {
                Messages.Dequeue();
            }
            Messages.Enqueue(message);
        }
    }

    public class Message
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
