using System;
using System.Drawing;

namespace WindowsGUI
{
    public class Shape
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
            color = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255)); //red
        }

        public void Move()
        {
            xAnchor += xVelocity;
            yAnchor += yVelocity;
        }

        public void Draw(Graphics g)
        {
            Console.WriteLine("Draw function was called from the Shape class... Nothing happened.");
        }
    }
}
