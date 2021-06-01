using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFinalCodeDrone
{
    public record ESCConfig
    {
        public int ESC1Pulse { get; init; }
        public int ESC2Pulse { get; init; }
        public int ESC3Pulse { get; init; }
        public int ESC4Pulse { get; init; }
        public ESCConfig(int samePulse)
        {
            ESC1Pulse = samePulse;
            ESC2Pulse = samePulse;
            ESC3Pulse = samePulse;
            ESC4Pulse = samePulse;
        }
        public ESCConfig()
        {

        }

        public ESCConfig(int eSC1Pulse, int eSC2Pulse, int eSC3Pulse, int eSC4Pulse) 
        {
            ESC1Pulse = eSC1Pulse;
            ESC2Pulse = eSC2Pulse;
            ESC3Pulse = eSC3Pulse;
            ESC4Pulse = eSC4Pulse;
        }
    }
}
