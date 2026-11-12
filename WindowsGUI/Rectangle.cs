using System;
using System.Collections.Generic;
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
            height = h; width = w;
        }

        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(color);
            g.FillRectangle(brush, xAnchor, yAnchor, width, height);
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            if (xAnchor <= 0)
            {
                xAnchor = 1;
                xVelocity *= -1;
            } 
            else if (xAnchor + width >= windowWidth)
            {
                xAnchor = windowWidth - width - 1;
                xVelocity *= -1;
            }

            if (yAnchor <= 0)
            {
                yAnchor = 1;
                yVelocity *= -1;
            }
            else if (yAnchor + height >= windowHeight)
            {
                yAnchor = windowHeight - height - 1;
                yVelocity *= -1;
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

            return false;
        }

        public override bool CollidesWithTriangle(Shape other)
        {
            throw new NotImplementedException();
        }

        public override PointF[] GetVertices()
        {
            throw new NotImplementedException();
        }

    }
}
