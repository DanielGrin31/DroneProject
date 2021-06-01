using Iot.Device.Bmxx80;
using Iot.Device.Common;
using Iot.Device.Imu;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace ProjectFinalCodeDrone
{
    public class IMUSensorsManager : IIMUSensorsManager
    {
        Mpu6050 mpu6050;
        Pressure defaultSeaPressure;
        Bmp280 bmp280;
        Length stationHeight;
        public IMUSensorsManager(Pressure pressure = default)
        {
            defaultSeaPressure = pressure == default ? WeatherHelper.MeanSeaLevel : pressure;

            I2cConnectionSettings i2cBmpSettings = new I2cConnectionSettings(1, 0x76);
            I2cDevice bmpi2cDevice = I2cDevice.Create(i2cBmpSettings);
            bmp280 = new Bmp280(bmpi2cDevice);
            bmp280.TemperatureSampling = Sampling.LowPower;
            bmp280.PressureSampling = Sampling.UltraHighResolution;
            SetGroundHeight();
            I2cConnectionSettings mpui2CConnectionSettingmpus = new(1, 0x68);//(הגדרת הגדרות המכשיר(כתובת ובאס ליין
            I2cDevice mpui2cDevice = I2cDevice.Create(mpui2CConnectionSettingmpus);
            mpu6050 = new Mpu6050(mpui2cDevice);

            GetGroundHeight();
        }
        public Length GetGroundHeight()
        {
            return stationHeight;
        }
        private void SetGroundHeight()
        {
            var readResult = bmp280.Read();
            var altitude = WeatherHelper.CalculateAltitude((Pressure)readResult.Pressure, defaultSeaPressure, (Temperature)readResult.Temperature);
            stationHeight = altitude;
        }

        public async Task<IMUReadResult> GetMeasurementsAsync()
        {
            var barometric = GetAltitudeAsync();
            var gyro = mpu6050.GetGyroscopeReading();
            var acc = mpu6050.GetAccelerometer();

            var altitude = await barometric;
            IMUReadResult result = new IMUReadResult(gyro, acc, altitude);
            return result;
        }
        public async Task<Length> GetAltitudeAsync()
        {
            var readResult = await bmp280.ReadAsync();
            var altitude = WeatherHelper.CalculateAltitude((Pressure)readResult.Pressure, defaultSeaPressure, (Temperature)readResult.Temperature);
            return altitude;
        }
    }
}
