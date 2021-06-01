using System.Threading.Tasks;
using UnitsNet;

namespace ProjectFinalCodeDrone
{
    public interface IIMUSensorsManager
    {
        Task<Length> GetAltitudeAsync();
        Length GetGroundHeight();
        Task<IMUReadResult> GetMeasurementsAsync();
    }
}