// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Spi;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Iot.Device.Common;
using Iot.Device.Imu;
using Iot.Device.Media;
using Iot.Device.Nrf24l01;
using UnitsNet;

#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable S1118 // Utility classes should not have public constructors
public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
#pragma warning restore S3903 // Types should be defined in named namespaces
{
    public static void Main(string[] args)
    {
        Video();
    }

    private static void Video()
    {
        VideoConnectionSettings settings = new(11, (1080, 1920), PixelFormat.H264);
        VideoDevice device = VideoDevice.Create(settings);

        // Get the supported formats of the device
        foreach (PixelFormat item in device.GetSupportedPixelFormats())
        {
            Console.Write($"{item} ");
        }

        Console.WriteLine();

        // Get the resolutions of the format
        foreach (var resolution in device.GetPixelFormatResolutions(PixelFormat.YUYV))
        {
            Console.Write($"[{resolution.MinWidth}x{resolution.MinHeight}]->[{resolution.MaxWidth}x{resolution.MaxHeight}], Step [{resolution.StepWidth},{resolution.StepHeight}] ");
        }

        Console.WriteLine();

        // Query v4l2 controls default and current value
        VideoDeviceValue value = device.GetVideoDeviceValue(VideoDeviceValueType.Rotate);
        Console.WriteLine($"{value.Name} Min: {value.Minimum} Max: {value.Maximum} Step: {value.Step} Default: {value.DefaultValue} Current: {value.CurrentValue}");

        string path = Directory.GetCurrentDirectory();

        // Take photos
        device.Capture($"{path}/jpg_direct_output.jpg");

        // Change capture setting
        device.Settings.PixelFormat = PixelFormat.YUV420;

        // Convert pixel format
        Color[] colors = VideoDevice.Yv12ToRgb(device.Capture(), settings.CaptureSize);
        Bitmap bitmap = VideoDevice.RgbToBitmap(settings.CaptureSize, colors);
        bitmap.Save($"{path}/yuyv_to_jpg.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
         
    }

    private static void Blink()
    {
        Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
        int pin = 18;
        using var controller = new GpioController();
        controller.OpenPin(pin, PinMode.Output);
        bool ledOn = true;
        while (true)
        {
            controller.Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
            Thread.Sleep(1000);
            ledOn = !ledOn;
        }
    }
    private static void PWMTest()
    {
        //הודעה למשתמש
        Console.WriteLine("Hello PWM!");
        //שווה ל20% duty cycle,בתדר 400 הרץ gpio 18בערוץ 0 אשר יוצא מ pwm יוצר ערוץ 
        var pwm = PwmChannel.Create(0, 0,400,0.2);
        //מתחיל את הערוץ
        pwm.Start();
        Console.Read();
    }
    private static void MPU6050Test()
    {
        Console.WriteLine("Hello Mpu6050!");//הודעה על תחילת התכנית

        I2cConnectionSettings mpui2CConnectionSettingmpus = new(1, 0x68);//(הגדרת הגדרות המכשיר(כתובת ובאס ליין

        I2cDevice i2cDevice = I2cDevice.Create(mpui2CConnectionSettingmpus);
        var mpu6050 = new Mpu6050(i2cDevice);
        while (true)
        {

            var gyro = mpu6050.GetGyroscopeReading();
            Console.WriteLine($"Gyro X = {gyro.X,15}");
            Console.WriteLine($"Gyro Y = {gyro.Y,15}");
            Console.WriteLine($"Gyro Z = {gyro.Z,15}");
            var acc = mpu6050.GetAccelerometer();
            Console.WriteLine($"Acc X = {acc.X,15}");
            Console.WriteLine($"Acc Y = {acc.Y,15}");
            Console.WriteLine($"Acc Z = {acc.Z,15}");
            Thread.Sleep(2000);
        }
    }

    public static string FormatNRFString(Nrf24l01 nrf)
    {
        string str = "";
        str += $"Channel:{nrf.Channel}\nAddress:{nrf.Address}\nData rate:{nrf.DataRate}\n{nrf.PowerMode}\n{nrf.WorkingMode}\n{nrf.OutputPower}\n";
        str += $"Pipes:\npipe0:{nrf.Pipe0}\n{nrf.Pipe1}\n{nrf.Pipe2}\n{nrf.Pipe3}\n{nrf.Pipe4}\n{nrf.Pipe5}";
        return str;
    }

private static void Receiver_ReceivedData(object sender, DataReceivedEventArgs e)

{
    var raw = e.Data;
    var msg = Encoding.UTF8.GetString(raw);

    Console.Write("Received Raw Data: ");
    foreach (var item in raw)
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine();

    Console.WriteLine($"Massage: {msg}");
    Console.WriteLine();
}

}