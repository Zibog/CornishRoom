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
                var cube = nearestFigure as Plane;
                normal = cube?.Normal;
            }
            
            // TODO: sphere

            var intensity = Intensity(intersectionPoint, normal);
            var mixedColor = MixColor(nearestFigure.Color, intensity);

            if (iteration == MaxDepth || nearestFigure.Material == Material.Matte)
                return mixedColor;

            Color tracedColor = Color.Black;

            if (nearestFigure.Material == Material.Reflective)
            {
                var reflectedRay = Helpers.Normalize(Reflect(to, normal));
                tracedColor = Trace(intersectionPoint, reflectedRay, ++iteration);
            }
            else
            {
                var transparencyRay = Helpers.Normalize(Refract(to, normal));
                tracedColor = Trace(intersectionPoint, transparencyRay, ++iteration);
            }

            const double ownColorCoefficient = 0.5;

            mixedColor = MixColorWithReflection(mixedColor, tracedColor, ownColorCoefficient);
            
            return mixedColor;
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
                var plane = figure as Plane;
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

        private static Color MixColorWithReflection(Color baseColor, Color reflectedColor, double ownColorCoefficient)
        {
            var mixedOwnColor = MixColor(baseColor, ownColorCoefficient);
            var mixedReflectedColor = MixColor(reflectedColor, 1 - ownColorCoefficient);
            return Color.FromArgb(
                mixedOwnColor.R + mixedReflectedColor.R,
                mixedOwnColor.G + mixedReflectedColor.G,
                mixedOwnColor.B + mixedReflectedColor.B
            );
        }

        private static Point3D Reflect(Point3D ray, Point3D normal)
        {
            return ray - 2 * Helpers.Scalar(ray, normal) * normal;
        }

        private static Point3D Refract(Point3D ray, Point3D normal)
        {
            // https://www.flipcode.com/archives/reflection_transmission.pdf
            var positiveNormal = Helpers.Normalize(normal);
            
            if (Helpers.Scalar(ray, normal) > 0)
                positiveNormal = Helpers.Normalize(-normal);

            var normalizedRay = Helpers.Normalize(ray);

            const double eta1 = 1.1;
            const double eta2 = 1.0;
            
            var cos = -Helpers.Scalar(positiveNormal, normalizedRay);
            var eta = cos > 0 ? eta1 / eta2 : eta2 / eta1;
            var sin = Math.Sqrt(Math.Max(1 - eta * eta * (1 - cos * cos), 0));

            return eta * normalizedRay + positiveNormal * (eta * cos - sin);
        }
    }
}