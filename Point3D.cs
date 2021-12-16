using System;

namespace CornishRoom
{
    public class Point3D
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        
        private const double Eps = 1E-10;
        
        public Point3D() {}

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static Point3D operator +(Point3D left, Point3D right)
        {
            return new Point3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
        
        public static Point3D operator -(Point3D left, Point3D right)
        {
            return left + -1 * right;
        }
        
        public static Point3D operator -(Point3D p)
        {
            return new Point3D(-p.X, -p.Y, -p.Z);
        }

        public static Point3D operator *(double value, Point3D right)
        {
            return new Point3D(right.X * value, right.Y * value, right.Z * value);
        }

        public static Point3D operator *(Point3D p, double value)
        {
            return value * p;
        }

        public static Point3D operator /(Point3D p, double value)
        {
            return new Point3D(p.X / value, p.Y / value, p.Z / value);
        }
        
        public static Point3D operator /(double value, Point3D p)
        {
            return p / value;
        }

        public bool BelongsTo(Plane plane)
        {
            // http://grafika.me/node/70
            var belongsByX = X < Math.Max(plane.To.X, plane.From.X) + Eps &&
                    X > Math.Min(plane.To.X, plane.From.X) - Eps;
            var belongsByY = Y < Math.Max(plane.To.Y, plane.From.Y) + Eps &&
                    Y > Math.Min(plane.To.Y, plane.From.Y) - Eps;
            var belongsByZ = Z < Math.Max(plane.To.Z, plane.From.Z) + Eps &&
                    Z > Math.Min(plane.To.Z, plane.From.Z) - Eps;

            return belongsByX && belongsByY && belongsByZ;
        }
    }
}