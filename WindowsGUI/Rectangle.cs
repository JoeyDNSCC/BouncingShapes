using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Text;

namespace WindowsGUI
{
    public class Rectangle : Shape
    {
        float height;
        float width;

        public Rectangle(float x, float y, float h, float w) : base(x, y)
        {
            height = h;
            width = w;

            // we'll say density is universally set to 1 for all shapes
            mass = width * height;

            //testing
            //mass = 1;

            momentOfInertia = mass * (width * width + height * height) / 12f;
        }

        public override void Draw(Graphics g)
        {
            GraphicsState state = g.Save();

            g.TranslateTransform(xAnchor, yAnchor);

            g.RotateTransform(rotation);

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, -width / 2, -height / 2, width, height);
            }

            g.Restore(state);
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;

            // rotation stuff
            float cos = (float)Math.Abs(Math.Cos(rotation * Math.PI / 180f));
            float sin = (float)Math.Abs(Math.Sin(rotation * Math.PI / 180f));

            float boundX = halfWidth * cos + halfHeight * sin;
            float boundY = halfWidth * sin + halfHeight * cos;

            
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

        public override bool CollidesWith(Shape other)
        {
            return other.CollidesWithRectangle(this);
        }

        public override bool CollidesWithCircle(Circle other)
        {
            throw new NotImplementedException();
        }

        public override bool CollidesWithRectangle(Rectangle other)
        {
            /*
            bool alignedOnX = (other.xAnchor + other.width >= xAnchor &&
                other.xAnchor <= xAnchor + width);

            bool alignedOnY = (other.yAnchor + other.height >= yAnchor &&
                other.yAnchor <= yAnchor + height);


            // refactor
            if (alignedOnX && alignedOnY) // they are touching
            {
                // whichever axis has smaller overlap is the touching side
                float overlapX = Math.Min(xAnchor + width, other.xAnchor + other.width) - Math.Max(xAnchor, other.xAnchor);
                
                float overlapY = Math.Min(yAnchor + height, other.yAnchor + other.height) - Math.Max(yAnchor, other.yAnchor);

                if (overlapX < overlapY) 
                {
                    // Horizontal collision
                    if (xAnchor < other.xAnchor)
                    {
                        xAnchor -= overlapX / 2 + 0.01f;
                        other.xAnchor += overlapX / 2 + 0.01f;
                    }
                    else
                    {
                        xAnchor += overlapX / 2 + 0.01f;
                        other.xAnchor -= overlapX / 2 + 0.01f;
                    }

                    // Velocity swap (only on x axis)
                    xVelocity *= -1;
                    other.xVelocity *= -1;
                }
                else
                {
                    // vertical collision
                    if (yAnchor < other.yAnchor)
                    {
                        yAnchor -= overlapY / 2 + 0.01f;
                        other.yAnchor += overlapY / 2 + 0.01f;
                    }
                    else
                    {
                        yAnchor += overlapY / 2 + 0.01f;
                        other.yAnchor -= overlapY / 2 + 0.01f;
                    }

                    yVelocity *= -1;
                    other.yVelocity *= -1;
                }


                return true;
            }
            */
            return false;
        }

        public override PointF[] GetVertices()
        {
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            // Convert rotation to radians
            float radians = rotation * (float)Math.PI / 180f;

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            // Vertices relative to the center
            PointF[] vertices =
            {
                new PointF(-halfWidth, -halfHeight),
                new PointF(halfWidth, -halfHeight),
                new PointF(halfWidth, halfHeight),
                new PointF(-halfWidth, halfHeight)
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

    }
}
