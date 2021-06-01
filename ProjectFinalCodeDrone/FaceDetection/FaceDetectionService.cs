using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ProjectFinalCodeDrone
{
    public class FaceDetectionService : IFaceDetectionService
    {
        public (Rectangle[] faces, Image<Rgb, byte> generatedImage) HaarCascade(Image<Rgb, byte> ImageCV)
        {
            try
            {
                Image<Rgb, byte> temp;
                Rectangle[] faces;
                string facePath = Path.GetFullPath(@"Data/haarcascade_frontalface_default.xml");
                (faces, temp) = FaceDetection(ImageCV, facePath);

                return (faces, temp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static (Rectangle[] faces, Image<Rgb, byte> generatedImage) FaceDetection(Image<Rgb, byte> ImageCV, string facePath)
        {
            var temp = ImageCV.Clone();
            Rectangle[] faces = CascadeDetector.Detect(ImageCV, facePath);

            foreach (var face in faces)
            {
                temp.Draw(face, new Rgb(0, 0, 255), 2);
            }
            return (faces, temp);
        }

    }
}
