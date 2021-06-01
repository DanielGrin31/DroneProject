using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Nrf24l01;
using ProjectFinalCodeDrone.FaceDetection;
using ProjectFinalCodeDrone.Thrust;
using DataReceivedEventArgs = Iot.Device.Nrf24l01.DataReceivedEventArgs;

namespace ProjectFinalCodeDrone
{
    public class DroneController : IDroneController
    {
        private readonly IIMUSensorsManager imu;
        private readonly IRFManager rf;
        private readonly IThrustController thrust;
        private readonly IFaceDetectionManager faceDetection;
        public static DroneMode droneMode = DroneMode.Unknown;
        public static ThrustLevel thrustLevel;
        public static FlightDirection flightDirection;
        public static bool Stop;
        public DroneController(IIMUSensorsManager imu, IRFManager rf, IThrustController thrust, IFaceDetectionManager faceDetection)
        {
            this.imu = imu;
            this.rf = rf;
            rf.dataReceived += Rf_dataReceived;
            this.thrust = thrust;
            this.faceDetection = faceDetection;
        }

        private void Rf_dataReceived(object sender, DataReceivedEventArgs e)
        {
            var raw = e.Data;
            var msg = Encoding.UTF8.GetString(raw);
            MessageHandler.Handle(msg);
        }

        public async Task StartAsync()
        {
            rf.Send("Mode");
            while (droneMode == DroneMode.Unknown)
            {
                Thread.Sleep(1000);
            }
            switch (droneMode)
            {
                case DroneMode.Joystick:
                    await JoystickModeAsync();
                    break;
                case DroneMode.FaceDetection:
                    await FaceDetectionModeAsync();
                    break;
            }
        }

        private async Task FaceDetectionModeAsync()
        {
            await thrust.AscendAsync();
            while (Stop == false)
            {
                await SendSensorsDataAsync();
                var faces = faceDetection.GetFaces();
                var detectedface = faces.First();
                Point RectCenter = new(detectedface.X + (detectedface.Width / 2), detectedface.Y - detectedface.Height / 2);
                Point ImgCenter = new Point(faceDetection.Width / 2, faceDetection.Height / 2);
                await AdjustThrustYawRollAsync(RectCenter, ImgCenter);
                await AdjustThrustPitch(detectedface.Height, detectedface.Width);
            }
            await thrust.DescendAsync();
        }

        private async Task AdjustThrustPitch(int height, int width)
        {
            if (height * width >= 600 * 600)
            {
                await thrust.PitchAsync(Thrust.PitchDirection.Backwards);
            }
            else if (height * width <= 300 * 300)
            {
                await thrust.PitchAsync(Thrust.PitchDirection.Forwards);
            }
        }

        private async Task AdjustThrustYawRollAsync(Point RectCenter, Point ImgCenter)
        {
            if (RectCenter.X >= ImgCenter.X + 100)
            {
                //move left
                int difference = RectCenter.X - ImgCenter.X;
                if (difference > 400)
                {
                    await thrust.RotateAsync(Thrust.RotateDirection.Left);
                }
                else
                {
                    await thrust.RollAsync(Thrust.RollDirection.Left);
                }
            }
            else if (RectCenter.X <= ImgCenter.X - 100)
            {
                int difference = ImgCenter.X - RectCenter.X;
                if (difference > 400)
                {
                    await thrust.RotateAsync(Thrust.RotateDirection.Right);
                }
                else
                {
                    await thrust.RollAsync(Thrust.RollDirection.Right);
                }
            }
        }

        private async Task SendSensorsDataAsync()
        {
            var result = await imu.GetMeasurementsAsync();
            var acc = result.acc;
            rf.Send($"{acc.X}|{acc.Y}|{acc.Z},{result.altitude.Meters}");
        }

        private async Task JoystickModeAsync()
        {
            while (Stop == false)
            {
                await SendSensorsDataAsync();
                if (thrustLevel == ThrustLevel.Float)
                {
                    switch (flightDirection)
                    {
                        case FlightDirection.Forwards:
                            await thrust.PitchAsync(PitchDirection.Forwards);
                            break;
                        case FlightDirection.BackWards:
                            await thrust.PitchAsync(PitchDirection.Backwards);
                            break;
                        case FlightDirection.Left:
                            await thrust.RollAsync(RollDirection.Left);
                            break;
                        case FlightDirection.Right:
                            await thrust.RollAsync(RollDirection.Right);
                            break;
                        case FlightDirection.None:
                            thrust.Float();
                            break;
                    }
                }
                switch (thrustLevel)
                {
                    case ThrustLevel.DescendFast:
                        thrust.WritePulseWidth(1150);
                        break;
                    case ThrustLevel.DescendSlow:
                        thrust.WritePulseWidth(1275);
                        break;
                    case ThrustLevel.AscendSlow:
                        thrust.WritePulseWidth(1500);
                        break;
                    case ThrustLevel.AscendFast:
                        thrust.WritePulseWidth(1600);
                        break;
                }
            }
        }
    }
}
