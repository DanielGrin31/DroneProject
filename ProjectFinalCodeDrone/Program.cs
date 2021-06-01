using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectFinalCodeDrone.FaceDetection;
using System;
using System.Threading.Tasks;

namespace ProjectFinalCodeDrone
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddSingleton<IRFManager, RFManager>();
                  services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
                  services.AddSingleton<IThrustController, ThrustController>();
                  services.AddSingleton<IIMUSensorsManager, IMUSensorsManager>();
                  services.AddSingleton<IFaceDetectionManager, FaceDetectionManager>();
                  services.AddSingleton<IDroneController, DroneController>();
                  
              });
            var host = builder.Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {

                    var app = services.GetRequiredService<DroneController>();
                    await app.StartAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error Occured, {e.Message}");
                }
            }
        }

    }
}
