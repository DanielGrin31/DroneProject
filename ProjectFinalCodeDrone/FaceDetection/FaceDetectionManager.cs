using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFinalCodeDrone.FaceDetection
{
    public class FaceDetectionManager : IFaceDetectionManager
    {
        IFaceDetectionService faceDetection;
        VideoCapture capture;

        public int Width { get; set; }

        public int Height { get; set; }

        public FaceDetectionManager(IFaceDetectionService faceDetection)
        {
            this.faceDetection = faceDetection;
            Init();
        }
        public Rectangle[] GetFaces()
        {
            var result = CaptureImage();
            if (result == default)
            {
                return null;
            }
            return result.faces;
        }
        private void Init()
        {
            capture = new VideoCapture(0);
             Width = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth));
             Height = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight));
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Output2.avi");
            Console.WriteLine(path);
        }
        private (Rectangle[] faces, Image<Rgb, byte> generatedImage) CaptureImage()
        {
            if (capture == null)
            {
                return default;
            }

            Mat m = new Mat();
            capture.Read(m);

            var result = faceDetection.HaarCascade(m.ToImage<Rgb, byte>());
            return result;
        }
    }

}
