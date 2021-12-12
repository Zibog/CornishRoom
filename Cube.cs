using System.Drawing;

namespace CornishRoom
{
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
}