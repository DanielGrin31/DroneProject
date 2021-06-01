using ProjectFinalCodeDrone.Thrust;
using System;

namespace ProjectFinalCodeDrone
{
    public class MessageHandler
    {
        public static void Handle(string msg)
        {
            if (msg.ToLower().Contains("stop"))
            {
                DroneController.Stop = true;
            }
            if (msg.Contains(","))
            {
                var args=msg.Split(',');
                if(args[0].ToLower().Contains("tl"))
                {
                    int startIndex1 = args[0].IndexOf(':') + 1;
                    int startIndex2 = args[1].IndexOf(':') + 1;

                    var thrustLevel = args[0][startIndex1..(args[0].Length - 1)];
                    var flightDirection= args[1][startIndex1..(args[1].Length - 1)];

                    DroneController.flightDirection = (FlightDirection)int.Parse(flightDirection);
                    DroneController.thrustLevel= (ThrustLevel)int.Parse(thrustLevel);
                }
            }
            else if(msg.ToLower().Contains("mode"))
            {
                int startIndex = msg.IndexOf(':') + 1;
                var mode=msg[startIndex..(msg.Length-1)];
                switch (mode.ToLower())
                {
                    case "joystick":
                        DroneController.droneMode = DroneMode.Joystick;
                        break;
                    case "facedetection":
                        DroneController.droneMode = DroneMode.FaceDetection;
                        break;
                }
            }
        }
    }
}