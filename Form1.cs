using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CornishRoom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            _figures = new List<Figure>();
            InitRoom(25, 25, 50);

            var lights = new List<Light>();
            lights.Add(new Light(new Point3D(0, 0, -20), 0.7));

            var position = new Point3D(0, -10, -49);

            var rt = new RayTracing(position, _figures, lights);

            pictureBox1.Image = rt.GetImage(pictureBox1.Width, pictureBox1.Height);
        }

        private List<Figure> _figures;

        private void InitRoom(double x, double y, double z)
        {
            // Задняя стена
            InitWall(Material.Matte, Color.Violet, new Point3D(-x, -y, z), new Point3D(x, y, z), new Point3D(0, 0, -1));
            // Пол
            InitWall(Material.Matte, Color.White, new Point3D(-x, -y, -z), new Point3D(x, -y, z), new Point3D(0, 1, 0));
            // Потолок
            InitWall(Material.Matte, Color.DarkMagenta, new Point3D(-x, y, -z), new Point3D(x, y, z), new Point3D(0, -1, 0));
            // Левая стена
            InitWall(Material.Matte, Color.LightBlue, new Point3D(-x, -y, -z), new Point3D(-x, y, z), new Point3D(1, 0, 0));
            // Правая стена
            InitWall(Material.Matte, Color.IndianRed, new Point3D(x, -y, -z), new Point3D(x, y, z), new Point3D(-1, 0, 0));
            // Передняя стена
            InitWall(Material.Matte, Color.LightPink, new Point3D(-x, -y, -z), new Point3D(x, y, -z), new Point3D(0, 0, 1));
        }
        
        private void InitWall(Material material, Color color, Point3D from, Point3D to, Point3D normal)
        {
            _figures.Add(new Plane(FigureType.Wall, material, color, from, to, normal));
        }

        private void InitCube(Point3D center, double distanceToPlane, Material material, Color color)
        {
            var p1 = center - distanceToPlane;
            var p2 = center + distanceToPlane;
        }
    }
}