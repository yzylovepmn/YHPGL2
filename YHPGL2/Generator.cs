using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public static double ToRadian(double angle)
        {
            return angle / 180 * Math.PI;
        }

        public static double ToAngle(double radian)
        {
            return radian * 180 / Math.PI;
        }

        public static IEnumerable<Vector> GenerateArcs(double startRadian, double sweepRadian, double chordRadian)
        {
            var currentRadian = startRadian;
            var endRadian = startRadian + sweepRadian;
            if (sweepRadian < 0)
                chordRadian = -chordRadian;

            while (true)
            {
                yield return new Vector(Math.Cos(currentRadian), Math.Sin(currentRadian));
                currentRadian += chordRadian;
                if ((chordRadian > 0 && currentRadian >= endRadian) || (chordRadian < 0 && currentRadian <= endRadian))
                    break;
            }
            yield return new Vector(Math.Cos(endRadian), Math.Sin(endRadian));
        }

        public static Point? CalcCenterByThreePoints(Point p1, Point p2, Point p3)
        {
            var p12 = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            var p13 = new Point((p1.X + p3.X) / 2, (p1.Y + p3.Y) / 2);
            var v12 = new Vector(p1.Y - p2.Y, p2.X - p1.X);
            var v13 = new Vector(p1.Y - p3.Y, p3.X - p1.X);

            var value = v13.Y * v12.X - v12.X * v13.Y;
            if (value == 0) return null;

            var m1 = v12.Y * p12.X - v12.X * p12.Y;
            var m2 = v13.Y * p13.X - v13.X * p13.Y;

            var x = (v13.X * m1 - v12.X * m2) / value;
            var y = (v13.Y * m1 - v12.Y * m2) / value;

            return new Point(x, y);
        }
    }
}