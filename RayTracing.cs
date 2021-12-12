using System.Collections.Generic;
using System.Drawing;

namespace CornishRoom
{
    public class RayTracing
    {
        private Point3D _position;
        private List<Figure> _objects;
        private List<Light> _lights;

        public RayTracing(Point3D position, List<Figure> objects, List<Light> lights)
        {
            _position = position;
            _objects = objects;
            _lights = lights;
        }

        private Color Trace(Point3D from, Point3D to, int iteration)
        {
            (Figure nearestFigure, double distance) = FindIntersection(from, to);
            
            return Color.Black;
        }

        private (Figure, double) FindIntersection(Point3D from, Point3D to)
        {
            Figure nearestFigure = null;
            var distance = double.MaxValue;

            foreach (var figure in _objects)
            {
                var distanceToFigure = Intersect(figure, from, to);
            }

            return (nearestFigure, distance);
        }

        private double Intersect(Figure figure, Point3D from, Point3D to)
        {
            if (figure.FigureType is FigureType.Cube or FigureType.Wall)
            {
                var plane = figure as Cube;
                var normal = Helpers.Normalize(plane.Normal);

                var distance = -Helpers.Scalar(from - plane.To, normal) / Helpers.Scalar(to, normal);

                if (distance < double.Epsilon)
                    return double.MaxValue;
                
                
            }
            
            // TODO: sphere

            return double.MaxValue;
        }
    }
}