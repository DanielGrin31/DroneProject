using Iot.Device.Nrf24l01;
using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFinalCodeDrone
{
    public class RFManager : IRFManager
    {
        public event EventHandler<DataReceivedEventArgs> dataReceived;
        private Nrf24l01 nrf;
        public RFManager(string address="NRF24")
        {
            SpiConnectionSettings settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = Nrf24l01.SpiClockFrequency,
                Mode = Nrf24l01.SpiMode
            };
            var device = SpiDevice.Create(settings);
            nrf = new Nrf24l01(device, 17, 18, 20);
            byte[] byteAddress = Encoding.UTF8.GetBytes(address);
            nrf.Address = byteAddress;
            nrf.Pipe0.Address = byteAddress;
            nrf.DataReceived += Nrf_DataReceived;
        }

        private void Nrf_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (dataReceived!=null)
            {
                dataReceived(sender, e);
            }
        }

        public void Send(string message)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            nrf.Send(byteMessage);
        }
      
    }
}
