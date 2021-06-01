using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ProjectFinalCodeDrone
{
    public static class CascadeDetector
    {
        public static Rectangle[] Detect(Image<Rgb,byte> ImageCV,string path)
        {
            CascadeClassifier classifierFace = new CascadeClassifier(path);
            var gray = ImageCV.Convert<Gray, byte>().Clone();
            return classifierFace.DetectMultiScale(gray, 1.1, 4);
        }
    }
}
