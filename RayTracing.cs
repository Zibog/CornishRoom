using System;
using System.Collections.Generic;
using System.Drawing;

// https://habr.com/ru/post/187720/
// http://www.graph.unn.ru/rus/materials/CG/CG15_RayTracing.pdf

namespace CornishRoom
{
    public class RayTracing
    {
        private Point3D _position;
        private List<Figure> _objects;
        private List<Light> _lights;

        private const byte MaxDepth = 5;

        public RayTracing(Point3D position, List<Figure> objects, List<Light> lights)
        {
            _position = position;
            _objects = objects;
            _lights = lights;
        }

        private Color Trace(Point3D from, Point3D to, int iteration)
        {
            var (nearestFigure, distance) = FindIntersection(from, to);
            
            if (nearestFigure == null)
                return Color.Black;

            var intersectionPoint = from + distance * to;

            Point3D normal = null;
            if (nearestFigure.FigureType is FigureType.Cube or FigureType.Wall)
            {
                var cube = nearestFigure as Cube;
                normal = cube?.Normal;
            }
            
            // TODO: sphere

            var intensity = Intensity(intersectionPoint, normal);
            var mixedColor = MixColor(nearestFigure.Color, intensity);

            if (iteration == MaxDepth || nearestFigure.Material == Material.Matte)
                return mixedColor;
            
            // TODO: recursion
            
            return Color.Black;
        }

        private (Figure, double) FindIntersection(Point3D from, Point3D to)
        {
            Figure nearestFigure = null;
            var distance = double.MaxValue;

            foreach (var figure in _objects)
            {
                var distanceToFigure = Intersect(figure, from, to);

                if (distanceToFigure < distance)
                    (nearestFigure, distance) = (figure, distanceToFigure);
            }

            return (nearestFigure, distance);
        }

        private static double Intersect(Figure figure, Point3D from, Point3D to)
        {
            if (figure.FigureType is FigureType.Cube or FigureType.Wall)
            {
                var plane = figure as Cube;
                var normal = Helpers.Normalize(plane.Normal);

                var distance = -Helpers.Scalar(from - plane.To, normal) / Helpers.Scalar(to, normal);

                if (distance < double.Epsilon)
                    return double.MaxValue;

                if ((from + distance * to).BelongsTo(plane))
                    return distance;
                
                return double.MaxValue;
            }
            
            // TODO: sphere

            return double.MaxValue;
        }

        private double Intensity(Point3D intersection, Point3D normal)
        {
            double intensity = 0;

            foreach (var light in _lights)
            {
                var destination = light.Position - intersection;
                var (nearestFigure, distance) = FindIntersection(intersection, destination);

                // Вторым условием отсекаем случаи, когда находимся внутри сферы
                if (nearestFigure.FigureType != FigureType.Cube && distance <= 1.0)
                    continue;
                
                var lightCoefficient = Helpers.Scalar(destination, normal);

                if (lightCoefficient > 0)
                    intensity += light.Intensity * lightCoefficient /
                                 (Helpers.Length(destination) * Helpers.Length(normal));
            }
            
            return intensity;
        }

        private static Color MixColor(Color color, double intensity)
        {
            var red = Math.Min((int)(color.R * intensity), byte.MaxValue);
            var green = Math.Min((int)(color.G * intensity), byte.MaxValue);
            var blue = Math.Min((int)(color.B * intensity), byte.MaxValue);
            return Color.FromArgb(red, green, blue);
        }
    }
}