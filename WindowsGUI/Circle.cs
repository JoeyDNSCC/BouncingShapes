using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGUI
{
    public class Circle : Shape
    {
        private float radius;
        private PointF[] hitbox;
        public Circle(float x, float y, float r) : base(x, y)
        {
            xAnchor = x;
            yAnchor = y;

            radius = r;
            
            int accuracy = 16; // num sides of hitbox
            hitbox = new PointF[accuracy];

            for (int i = 0; i < accuracy; i++)
            {
                float angle = rotation + (2 * MathF.PI * i / accuracy);

                hitbox[i] = new PointF(
                    radius * MathF.Cos(angle),
                    radius * MathF.Sin(angle)
                );
            }
            
            mass = (float)Math.PI * radius * radius;

            momentOfInertia =
                0.5f * mass * radius * radius;



        }

        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(color);
            // add radius to center on cursor
            g.FillEllipse(brush, xAnchor - radius, yAnchor - radius, radius * 2, radius * 2);
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            if (xAnchor - radius < 0)
            {
                xAnchor = radius;
                xVelocity = Math.Abs(xVelocity);
            }
            else if (xAnchor + radius > windowWidth)
            {
                xAnchor = windowWidth - radius;
                xVelocity = -Math.Abs(xVelocity);
            }

            if (yAnchor - radius < 0)
            {
                yAnchor = radius;
                yVelocity = Math.Abs(yVelocity);
            }
            else if (yAnchor + radius > windowHeight)
            {
                yAnchor = windowHeight - radius;
                yVelocity = -Math.Abs(yVelocity);
            }
        }


        public override bool CollidesWith(Shape other)
        {
            return other.CollidesWithCircle(this);
        }

        public override bool CollidesWithCircle(Circle other)
        {
            float dx = other.xAnchor - xAnchor;
            float dy = other.yAnchor - yAnchor;

            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // if not collided, exit
            if (distance >= radius + other.radius)
                return false;

            // if div by 0, fudge the numbers a bit
            if (distance == 0)
            {
                dx = 1;
                dy = 0;
                distance = 1;
            }

            #region Separate

            float overlap = (radius + other.radius) - distance;

            float normalX = dx / distance;
            float normalY = dy / distance;

            float separationX = normalX * overlap / 2;
            float separationY = normalY * overlap / 2;

            xAnchor -= separationX;
            yAnchor -= separationY;

            other.xAnchor += separationX;
            other.yAnchor += separationY;

            #endregion

            #region Swap Velocity

            float tempX = xVelocity;
            float tempY = yVelocity;

            xVelocity = other.xVelocity;
            yVelocity = other.yVelocity;

            other.xVelocity = tempX;
            other.yVelocity = tempY;

            #endregion

            return true;
        }

        public override bool CollidesWithRectangle(Rectangle other)
        {
            throw new NotImplementedException();
        }


        public override PointF[] GetVertices()
        {
            PointF[] translated = new PointF[hitbox.Length];

            float radians = rotation * (float)Math.PI / 180f;

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            for (int i = 0; i < hitbox.Length; i++)
            {
                float rotatedX = hitbox[i].X * cos - hitbox[i].Y * sin;
                float rotatedY = hitbox[i].X * sin + hitbox[i].Y * cos;

                translated[i] = new PointF(
                    rotatedX + xAnchor,
                    rotatedY + yAnchor
                );
            }

            return translated;
        }
    }
}
