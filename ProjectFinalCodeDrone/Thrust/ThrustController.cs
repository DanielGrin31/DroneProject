using Iot.Device.ServoMotor;
using ProjectFinalCodeDrone.Thrust;
using System;
using System.Collections.Generic;
using System.Device.Pwm.Drivers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet;

namespace ProjectFinalCodeDrone
{
    public class ThrustController : IThrustController
    {
        ServoMotor ESC1;
        ServoMotor ESC2;
        ServoMotor ESC3;
        ServoMotor ESC4;
        ESCConfig currentConfig;
        const int floatPulse = 1350;
        private const int TurnPulse = 1400;
        private const int PitchLowPulse = 1325;
        private const int PitchHighPulse = 1375;
        private const int RollHighPulse = 1375;
        private const int RollLowPulse = 1325;

        IMUSensorsManager imu;
        public ThrustController(IMUSensorsManager imu)
        {
            this.imu = imu;
            var pwm1 = new SoftwarePwmChannel(13, frequency: 50, dutyCycle: 0);
            var pwm2 = new SoftwarePwmChannel(16, frequency: 50, dutyCycle: 0);
            var pwm3 = new SoftwarePwmChannel(19, frequency: 50, dutyCycle: 0);
            var pwm4 = new SoftwarePwmChannel(20, frequency: 50, dutyCycle: 0);
            ESC1 = new ServoMotor(pwm1);
            ESC2 = new ServoMotor(pwm2);
            ESC3 = new ServoMotor(pwm3);
            ESC4 = new ServoMotor(pwm4);
            ESC1.Start();
            ESC2.Start();
            ESC3.Start();
            ESC4.Start();
        }
        public void WritePulseWidth(ESCConfig config)
        {
            currentConfig = config;
            ESC1.WritePulseWidth(config.ESC1Pulse);
            ESC2.WritePulseWidth(config.ESC2Pulse);
            ESC3.WritePulseWidth(config.ESC3Pulse);
            ESC4.WritePulseWidth(config.ESC4Pulse);
        }
        public void WritePulseWidth(int samePulse)
        {
            ESCConfig config = new ESCConfig(samePulse);
            WritePulseWidth(config);
        }
        public void Float()
        {
            WritePulseWidth(new ESCConfig(floatPulse));
        }
        public async Task AscendAsync()
        {
            if (currentConfig == null)
            {
                return;
            }
            var ground = imu.GetGroundHeight();
            bool isGrounded = (await imu.GetAltitudeAsync()) <= ground + Length.FromMeters(1);
            if (isGrounded)
            {
                Float();
                Thread.Sleep(1000);
                WritePulseWidth(TurnPulse);
                Thread.Sleep(1000);
                WritePulseWidth(1450);
                Thread.Sleep(1000);
                Float();
            }
        }
        public async Task DescendAsync()
        {
            if (currentConfig == null)
            {
                return;
            }
            Float();
            Thread.Sleep(1500);
            WritePulseWidth(1275);
            var ground = imu.GetGroundHeight();
            bool isGrounded = false;
            while (isGrounded == false)
            {
                var altitude = await imu.GetAltitudeAsync();
                if (altitude - Length.FromMeters(1) <= ground)
                {
                    isGrounded = true;
                }
            }
            WritePulseWidth(1000);
        }
        public Task RotateAsync(RotateDirection direction)
        {
            ESCConfig config = null;
            switch (direction)
            {
                case RotateDirection.Right:
                    config = new ESCConfig(TurnPulse, floatPulse, floatPulse, TurnPulse);
                    break;
                case RotateDirection.Left:
                    config = new ESCConfig(floatPulse, TurnPulse, TurnPulse, floatPulse);
                    break;
            }
            WritePulseWidth(config);
            return Task.CompletedTask;
        }
        public Task PitchAsync(PitchDirection direction)
        {
            ESCConfig config = null;
            switch (direction)
            {
                case PitchDirection.Forwards:
                    config = new ESCConfig(PitchLowPulse, PitchLowPulse, PitchHighPulse, PitchHighPulse);
                    break;
                case PitchDirection.Backwards:
                    config = new ESCConfig(PitchHighPulse, PitchHighPulse, PitchLowPulse, PitchLowPulse);
                    break;
            }
            WritePulseWidth(config);
            return Task.CompletedTask;
        }
        public Task RollAsync(RollDirection direction)
        {
            ESCConfig config = null;
            switch (direction)
            {
                case RollDirection.Left:
                    config = new ESCConfig(RollLowPulse, RollHighPulse, RollLowPulse, RollHighPulse);
                    break;
                case RollDirection.Right:
                    config = new ESCConfig(RollHighPulse, RollLowPulse, RollHighPulse, RollLowPulse);
                    break;
            }
            WritePulseWidth(config);
            return Task.CompletedTask;
        }
    }
}
