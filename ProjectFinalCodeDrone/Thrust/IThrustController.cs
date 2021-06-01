using ProjectFinalCodeDrone.Thrust;
using System.Threading.Tasks;

namespace ProjectFinalCodeDrone
{
    public interface IThrustController
    {
        Task AscendAsync();
        Task DescendAsync();
        void Float();
        Task PitchAsync(PitchDirection direction);
        Task RollAsync(RollDirection direction);
        Task RotateAsync(RotateDirection direction);
        void WritePulseWidth(ESCConfig config);
        void WritePulseWidth(int samePulse);
    }
}