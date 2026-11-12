using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGUI
{
    public abstract class Shape
    {
        private Random random = new Random();
        protected float xAnchor;
        protected float yAnchor;
        protected float xVelocity;
        protected float yVelocity;

        protected Color color;

        public Shape(float x, float y)
        {
            xAnchor = x;
            yAnchor = y;

            xVelocity = random.Next(0, 2) == 0
                ? random.Next(5, 16)
                : random.Next(-15, -4);
            yVelocity = random.Next(0, 2) == 0
                ? random.Next(5, 16)
                : random.Next(-15, -4);

            color = Color.FromArgb(255, random.Next(128,256), random.Next(128, 256), random.Next(128,256));
        }

        public void Move()
        {
            xAnchor += xVelocity;
            yAnchor += yVelocity;
        }

        public abstract void Draw(Graphics g);

        public abstract void DetectEdge(int windowWidth, int windowHeight);

        // "float dispatching"
        // CollidesWith() has the "other" call the correct collision method
        public abstract bool CollidesWith(Shape other);

        public abstract bool CollidesWithCircle(Circle other);
        public abstract bool CollidesWithRectangle(Rectangle other);
        public abstract bool CollidesWithTriangle(Shape other);



        public abstract PointF[] GetVertices();


        public float getXAnchor
        {
            get { return xAnchor; }
        }

        public float getYAnchor
        {
            get { return yAnchor; }
        }
    }
}
