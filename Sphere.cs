using System.Drawing;

namespace CornishRoom
{
    public class Sphere : Figure
    {
        public Point3D Center { get; }
        public double Radius { get; }
        
        public Sphere(Material material, Color color,
            Point3D center, double radius)
            : base(FigureType.Sphere, material, color)
        {
            Center = center;
            Radius = radius;
        }
    }
}