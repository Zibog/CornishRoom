﻿using System;
using System.Collections.Generic;
using System.Drawing;

// https://habr.com/ru/post/187720/
// http://www.graph.unn.ru/rus/materials/CG/CG15_RayTracing.pdf

namespace CornishRoom
{
    public class RayTracing
    {
        private readonly Point3D _position;
        private readonly List<Figure> _objects;
        private readonly List<Light> _lights;

        private const byte MaxDepth = 10;
        private const double Eps = 1E-10;

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

            Point3D normal;
            if (nearestFigure.FigureType is FigureType.Cube or FigureType.Wall)
            {
                var plane = nearestFigure as Plane;
                normal = plane?.Normal;
            }
            else
            {
                var sphere = nearestFigure as Sphere;
                normal = Helpers.Normalize(intersectionPoint - sphere?.Center);
            }

            var intensity = Intensity(intersectionPoint, normal);
            var mixedColor = MixColor(nearestFigure.Color, intensity);

            if (iteration == MaxDepth || nearestFigure.Material == Material.Matte)
                return mixedColor;

            Color tracedColor;

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

            const double ownColorCoefficient = 0.4;

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
            switch (figure.FigureType)
            {
                case FigureType.Cube or FigureType.Wall:
                {
                    var plane = figure as Plane;
                    var normal = Helpers.Normalize(plane!.Normal);

                    var distance = -Helpers.Scalar(@from - plane.To, normal) / Helpers.Scalar(to, normal);

                    if (distance < Eps)
                        return double.MaxValue;

                    return (@from + distance * to).BelongsTo(plane) ? distance : double.MaxValue;
                }
                case FigureType.Sphere:
                {
                    var sphere = figure as Sphere;
                    var centerToEye = from - sphere!.Center;

                    var a = Helpers.Scalar(to, to);
                    var b = 2 * Helpers.Scalar(centerToEye, to);
                    var c = Helpers.Scalar(centerToEye, centerToEye) - sphere!.Radius * sphere!.Radius;

                    var d = b * b - 4 * a * c;
                    
                    if (d < Eps)
                        return double.MaxValue;

                    var root1 = (-b + Math.Sqrt(d)) / (2 * a);
                    var root2 = (-b - Math.Sqrt(d)) / (2 * a);

                    if (Math.Max(root1, root2) < Eps)
                        return double.MaxValue;
                    
                    return root1 > Eps ? root2 : root1;
                }
                default:
                    return double.MaxValue;
            }
        }

        private double Intensity(Point3D intersection, Point3D normal)
        {
            double intensity = 0;

            foreach (var light in _lights)
            {
                var destination = light.Position - intersection;
                var (nearestFigure, distance) = FindIntersection(intersection, destination);

                if (nearestFigure.FigureType != FigureType.Wall && distance <= 1.0)
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

        public Bitmap GetImage(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var point = ScaledPoint(i, j, width, height);
                    bmp.SetPixel(i, j, Trace(_position, point, 0));
                }
            }

            return bmp;
        }
        
        private static Point3D ScaledPoint(int x, int y, int width, int height)
        {
            var scaledX = (x - width / 2) * (width / 100.0 / width);
            var scaledY = -(y - height / 2) * (height / 100.0 / height);
            return new Point3D(scaledX, scaledY, 4);
        }
    }
}