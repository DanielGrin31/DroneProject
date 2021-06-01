using System.Drawing;

namespace ProjectFinalCodeDrone.FaceDetection
{
    public interface IFaceDetectionManager
    {
        int Width { get; }
        int Height{ get; }
        Rectangle[] GetFaces();
    }
}