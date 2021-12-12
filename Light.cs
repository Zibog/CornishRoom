namespace CornishRoom
{
    public class Light
    {
        public Point3D Position { get; }
        public double Intensity { get; }

        public Light(Point3D position, double intensity)
        {
            Position = position;
            Intensity = intensity;
        }
    }
}