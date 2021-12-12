using System;

namespace CornishRoom
{
    public class Point3D
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        
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

        public bool BelongsTo(Cube plane)
        {
            throw new NotImplementedException();
        }
    }
}