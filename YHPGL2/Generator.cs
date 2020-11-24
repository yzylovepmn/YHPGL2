using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class Generator
    {
        public static double FormatAngle(double angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < -360)
                angle += 360;
            return angle;
        }

        public static double FormatChordAngle(double angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            if (angle < 0.5)
                angle = 0.5;
            if (angle > 180)
                angle = 360 - angle;
            return angle;
        }
    }
}