using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WindowsGUI
{
    public class Cube : Shape
    {
        public float size;
        public System.Drawing.Rectangle pictureBox;
        private Image picture = Image.FromFile("D:\\NSCC\\semester3\\advprog\\assignments\\WindowsGUI\\WindowsGUI\\Resources\\black-mage.png");
        public Cube(float x, float y, float s) : base(x, y) {
            size = s;
            mass = size * size; // no density
            // M(a^2)/2 where a = length of side
            momentOfInertia = mass * mass / 6f;
        }

        public override void Draw(Graphics g)
        {
            GraphicsState state = g.Save();

            g.TranslateTransform(xAnchor, yAnchor);
            g.RotateTransform(rotation);

            g.DrawImage(picture, -size / 2f, -size / 2f, (int)size, (int)size);

            g.Restore(state);
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {

            // figure out its rotation
            float cos = (float)Math.Abs(Math.Cos(rotation * Math.PI / 180f));
            float sin = (float)Math.Abs(Math.Sin(rotation * Math.PI / 180f));
            float boundX = (size / 2f) * cos + (size / 2f) * sin;
            float boundY = (size / 2f) * sin + (size / 2f) * cos;
            //Debug.WriteLine("BoundX: " + boundX);
            //Debug.WriteLine("BoundY: " + boundY);

            if (xAnchor - boundX < 0) // left
            {
                xAnchor = boundX;
                xVelocity = Math.Abs(xVelocity);
            }
            else if (xAnchor + boundX > windowWidth) // right
            {
                xAnchor = windowWidth - boundX;
                xVelocity = -Math.Abs(xVelocity);
            }

            if (yAnchor - boundY < 0) // top
            {
                yAnchor = boundY;
                yVelocity = Math.Abs(yVelocity);
            }
            else if (yAnchor + boundY > windowHeight) // bottom
            {
                yAnchor = windowHeight - boundY;
                yVelocity = -Math.Abs(yVelocity);
            }
        }

        public override PointF[] GetVertices()
        {
            float halfSize = size / 2f;

            // Convert rotation to radians
            float radians = rotation * (float)Math.PI / 180f;

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            // Vertices relative to the center
            PointF[] vertices =
            {
                new PointF(-halfSize, -halfSize),
                new PointF(halfSize, -halfSize),
                new PointF(halfSize, halfSize),
                new PointF(-halfSize, halfSize)
            };

            // Rotate and translate each vertex
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].X;
                float y = vertices[i].Y;

                vertices[i] = new PointF(
                    xAnchor + x * cos - y * sin,
                    yAnchor + x * sin + y * cos
                );
            }

            return vertices;
        }

        public override bool CollidesWith(Shape other)
        {
            throw new NotImplementedException();
        }

        public override bool CollidesWithCircle(Circle other)
        {
            throw new NotImplementedException();
        }

        public override bool CollidesWithRectangle(Rectangle other)
        {
            throw new NotImplementedException();
        }
    }
}
