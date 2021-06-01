using Iot.Device.Nrf24l01;
using System;

namespace ProjectFinalCodeDrone
{
    public interface IRFManager
    {
        void Send(string message);
         event EventHandler<DataReceivedEventArgs> dataReceived;

    }
}