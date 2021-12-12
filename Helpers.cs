using System;

namespace CornishRoom
{
    public static class Helpers
    {
        public static double Length(Point3D vector)
        {
            return Math.Sqrt(
                vector.X * vector.X
                + vector.Y * vector.Y
                + vector.Z * vector.Z
            );
        }
        
        public static Point3D Normalize(Point3D vector)
        {
            var length = Length(vector);
            return new Point3D(
                vector.X / length,
                vector.Y / length,
                vector.Z / length
                );
        }

        public static double Scalar(Point3D v1, Point3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
    }
}