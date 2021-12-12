using System.Drawing;

namespace CornishRoom
{
    public enum FigureType { Cube = 0, Sphere, Wall }
    
    public enum Material { Matte = 0, Transparent, Reflective }
    
    public abstract class Figure
    {
        public FigureType FigureType { get; }
        public Material Material { get; set; }
        public Color Color { get; set; }

        public Figure(FigureType type, Material material, Color color)
        {
            FigureType = type;
            Material = material;
            Color = color;
        }
    }

    public class Cube : Figure
    {
        public Point3D From { get; }
        public Point3D To { get; }
        public Point3D Normal { get; }

        public Cube(FigureType type, Material material, Color color,
            Point3D from, Point3D to, Point3D normal)
            : base(type, material, color)
        {
            From = from;
            To = to;
            Normal = normal;
        }
    }

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