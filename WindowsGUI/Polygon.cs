using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; 

namespace WindowsGUI
{
    public class Polygon : Shape
    {
        private PointF[] vertices;
        public Polygon(float x, float y, float r) : base(x, y)
        {
            Random random = Random.Shared;

            // make normal polygon with random sides and "radius"
            int numSides = random.Next(4, 11); 
            float radius = r;

            // Random spawn rotation for funsies
            float rotation = (float)(random.NextDouble() * Math.PI * 2);

            vertices = new PointF[numSides];

            for (int i = 0; i < numSides; i++)
            {
                float angle = rotation + (2 * MathF.PI * i / numSides);

                vertices[i] = new PointF(
                    radius * MathF.Cos(angle),
                    radius * MathF.Sin(angle)
                );
            }

            CenterVertices();

            // Area is mass bcuz no density
            mass = ShoelaceArea(vertices);

            momentOfInertia = CalculateMomentOfInertia(vertices, mass);
        }

        private void CenterVertices()
        {
            float centerX = (vertices.Min(v => v.X) + vertices.Max(v => v.X)) / 2f;
            float centerY = (vertices.Min(v => v.Y) + vertices.Max(v => v.Y)) / 2f;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new PointF(
                    (vertices[i].X - centerX),
                    (vertices[i].Y - centerY)
                );
            }
        }

        
        //https://rosettacode.org/wiki/Shoelace_formula_for_polygonal_area#C#
        static float ShoelaceArea(PointF[] v)
        {
            int n = v.Length;
            float a = 0f;
            for (int i = 0; i < n - 1; i++)
            {
                a += v[i].X * v[i + 1].Y - v[i + 1].X * v[i].Y;
            }
            return Math.Abs(a + v[n - 1].X * v[0].Y - v[0].X * v[n - 1].Y) / 2f;
        }
        static float CalculateMomentOfInertia(PointF[] v, float mass)
        {
            int n = v.Length;
            float numerator = 0f;
            float denominator = 0f;

            for (int i = 0; i < n; i++)
            {
                PointF p1 = v[i];
                PointF p2 = v[(i + 1) % n];

                float cross = p1.X * p2.Y - p2.X * p1.Y;

                numerator += cross * (
                    p1.X * p1.X +
                    p1.X * p2.X +
                    p2.X * p2.X +
                    p1.Y * p1.Y +
                    p1.Y * p2.Y +
                    p2.Y * p2.Y
                );

                denominator += cross;
            }

            // Polar mass moment of inertia about the origin
            return mass * numerator / (6f * denominator);
        }

        public override void Draw(Graphics g)
        {
            PointF[] translatedPoints = GetVertices();

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, translatedPoints);
            }
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            PointF[] points = GetVertices();
            
            // find leftmost and rightmost points
            float minX = points.Min(p => p.X);
            float maxX = points.Max(p => p.X);

            // highest & lowest points
            float minY = points.Min(p => p.Y);
            float maxY = points.Max(p => p.Y);

            if (minX <= 0)
            {
                xAnchor -= minX;
                xVelocity = Math.Abs(xVelocity);
            }
            else if (maxX >= windowWidth)
            {
                xAnchor -= maxX - windowWidth;
                xVelocity = -Math.Abs(xVelocity);
            }

            if (minY <= 0)
            {
                yAnchor -= minY;
                yVelocity = Math.Abs(yVelocity);
            }
            else if (maxY >= windowHeight)
            {
                yAnchor -= maxY - windowHeight;
                yVelocity = -Math.Abs(yVelocity);
            }
        }


        #region Double Dispatching
        public override bool CollidesWith(Shape other)
        {
            // Implement polygon collision detection here.
            return false;
        }

        public override bool CollidesWithCircle(Circle other)
        {
            return false;
        }

        public override bool CollidesWithRectangle(Rectangle other)
        {
            return false;
        }
        #endregion

        public override PointF[] GetVertices()
        {
            PointF[] translated = new PointF[vertices.Length];

            float radians = rotation * (float)Math.PI / 180f;

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            for (int i = 0; i < vertices.Length; i++)
            {
                float rotatedX = vertices[i].X * cos - vertices[i].Y * sin;
                float rotatedY = vertices[i].X * sin + vertices[i].Y * cos;

                translated[i] = new PointF(
                    rotatedX + xAnchor,
                    rotatedY + yAnchor
                );
            }

            return translated;
        }

    }
}