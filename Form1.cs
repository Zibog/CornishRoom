using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CornishRoom
{
    public partial class Form1 : Form
    {
        private readonly List<Figure> _figures;

        public Form1()
        {
            InitializeComponent();

            _figures = new List<Figure>();
            InitRoom(25, 25, 50);
            InitCube(Material.Transparent, Color.DarkKhaki, new Point3D(-15, -21, 20), 4);
            InitCube(Material.Reflective, Color.GreenYellow, new Point3D(10, -22, 0), 3);
            InitCube(Material.Matte, Color.Red, new Point3D(0, 0, 0), 5);
            InitSphere(Material.Reflective, Color.Cornsilk, new Point3D(10, -15, 15), 10);
            InitSphere(Material.Transparent, Color.Chartreuse, new Point3D(10, -15, 0), 3);
            InitSphere(Material.Matte, Color.Indigo, new Point3D(-15, -10, -25), 5);

            var lights = new List<Light>
            {
                new(new Point3D(0, 0, -20), 0.7),
                new(new Point3D(15, 20, 15), 0.3)
            };

            var position = new Point3D(0, -10, -49);

            var rt = new RayTracing(position, _figures, lights);

            pictureBox1.Image = rt.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void InitRoom(double x, double y, double z)
        {
            InitWall(Material.Matte, Color.Violet, new Point3D(-x, -y, z), new Point3D(x, y, z), new Point3D(0, 0, -1));
            InitWall(Material.Reflective, Color.White, new Point3D(-x, -y, -z), new Point3D(x, -y, z), new Point3D(0, 1, 0));
            InitWall(Material.Matte, Color.DarkMagenta, new Point3D(-x, y, -z), new Point3D(x, y, z), new Point3D(0, -1, 0));
            InitWall(Material.Matte, Color.LightBlue, new Point3D(-x, -y, -z), new Point3D(-x, y, z), new Point3D(1, 0, 0));
            InitWall(Material.Reflective, Color.IndianRed, new Point3D(x, -y, -z), new Point3D(x, y, z), new Point3D(-1, 0, 0));
            InitWall(Material.Matte, Color.LightPink, new Point3D(-x, -y, -z), new Point3D(x, y, -z), new Point3D(0, 0, 1));
        }
        
        private void InitWall(Material material, Color color, Point3D from, Point3D to, Point3D normal)
        {
            _figures.Add(new Plane(FigureType.Wall, material, color, from, to, normal));
        }

        private void InitCube(Material material, Color color, Point3D center, double distanceToPlane)
        {
            var p1 = center - distanceToPlane;
            var p2 = center + distanceToPlane;

            InitPlane(material, color, new Point3D(p1.X, p1.Y, p2.Z), new Point3D(p2.X, p2.Y, p2.Z), new Point3D(0, 0, 1));
            InitPlane(material, color, new Point3D(p1.X, p1.Y, p1.Z), new Point3D(p2.X, p2.Y, p1.Z), new Point3D(0, 0, -1));
            
            InitPlane(material, color, new Point3D(p1.X, p1.Y, p1.Z), new Point3D(p1.X, p2.Y, p2.Z), new Point3D(-1, 0, 0));
            InitPlane(material, color, new Point3D(p2.X, p1.Y, p1.Z), new Point3D(p2.X, p2.Y, p2.Z), new Point3D(1, 0, 0));
            
            InitPlane(material, color, new Point3D(p1.X, p1.Y, p1.Z), new Point3D(p2.X, p1.Y, p2.Z), new Point3D(0, -1, 0));
            InitPlane(material, color, new Point3D(p1.X, p2.Y, p1.Z), new Point3D(p2.X, p2.Y, p2.Z), new Point3D(0, 1, 0));
        }

        private void InitPlane(Material material, Color color, Point3D from, Point3D to, Point3D normal)
        {
            _figures.Add(new Plane(FigureType.Cube, material, color, from, to, normal));
        }

        private void InitSphere(Material material, Color color, Point3D center, double radius)
        {
            _figures.Add(new Sphere(material, color, center, radius));
        }
    }
}