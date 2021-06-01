using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;


namespace ProjectFinalCodeDrone
{
    public record IMUReadResult
    {
        public Vector3 gyro { get; init; }
        public Vector3 acc { get; init; }
        public Length altitude { get; init; }

        public IMUReadResult(Vector3 gyro, Vector3 acc, Length altitude)
        {
            this.gyro = gyro;
            this.acc = acc;
            this.altitude = altitude;
        }
    }
}
