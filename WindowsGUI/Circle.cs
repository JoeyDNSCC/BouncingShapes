using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGUI
{
    public class Circle : Shape
    {
        private float radius;

        public Circle(float x, float y, float r) : base(x, y)
        {
            xAnchor = x;
            yAnchor = y;

            radius = r;

        }

        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(color);
            // add radius to center on cursor
            g.FillEllipse(brush, xAnchor - radius, yAnchor - radius, radius * 2, radius * 2);
        }

        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            if (xAnchor - radius <= 0)
            {
                xAnchor = radius + 1;
                xVelocity *= -1;
            }
            else if (xAnchor + radius >= windowWidth)
            {
                xAnchor = windowWidth - radius - 1;
                xVelocity *= -1;
            }
            
            if (yAnchor - radius <= 0)
            {
                yAnchor = radius + 1;
                yVelocity *= -1;
            }
            else if (yAnchor + radius >= windowHeight)
            {
                yAnchor = windowHeight - radius - 1;
                yVelocity *= -1;
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
