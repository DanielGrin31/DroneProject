using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace ProjectFinalCodeDrone
{
    public interface IFaceDetectionService
    {
        (Rectangle[] faces, Image<Rgb, byte> generatedImage) HaarCascade(Image<Rgb, byte> ImageCV);
    }
}