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
            var x = (this.X < Math.Max(plane.To.X, plane.From.X) + Eps) &&
                    (this.X > Math.Min(plane.To.X, plane.From.X) - Eps);
            var y = (this.Y < Math.Max(plane.To.Y, plane.From.Y) + Eps) &&
                    (this.Y > Math.Min(plane.To.Y, plane.From.Y) - Eps);
            var z = (this.Z < Math.Max(plane.To.Z, plane.From.Z) + Eps) &&
                    (this.Z > Math.Min(plane.To.Z, plane.From.Z) - Eps);

            return x && y && z;
            // http://grafika.me/node/70
            var belongsByX = X > plane.From.X + Eps && X < plane.To.X + Eps;
            var belongsByY = Y > plane.From.Y + Eps && Y < plane.To.Y + Eps;
            var belongsByZ = Z > plane.From.Z + Eps && Z < plane.To.Z + Eps;
            return belongsByX && belongsByY && belongsByZ;
            
            return
            X < Math.Max(plane.To.X, plane.From.X) + double.Epsilon && X > Math.Min(plane.To.X, plane.From.X) - double.Epsilon &&
            Y < Math.Max(plane.To.Y, plane.From.Y) + double.Epsilon && Y > Math.Min(plane.To.Y, plane.From.Y) - double.Epsilon &&
            Z < Math.Max(plane.To.Z, plane.From.Z) + double.Epsilon && Z > Math.Min(plane.To.Z, plane.From.Z) - double.Epsilon;
        }
    }
}