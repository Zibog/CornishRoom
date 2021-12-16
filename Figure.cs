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

        protected Figure(FigureType type, Material material, Color color)
        {
            FigureType = type;
            Material = material;
            Color = color;
        }
    }
}