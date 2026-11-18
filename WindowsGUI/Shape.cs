using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGUI
{
    public abstract class Shape
    {
        private static Random random = new Random();

        // anchor point from which to draw
        protected float xAnchor;
        protected float yAnchor;

        // movement each frame
        protected float xVelocity;
        protected float yVelocity;

        protected float rotation; // current angle
        protected float angularVelocity; // rotational speed
        protected float mass; // resistance to linear acceleration
        protected float momentOfInertia; // resistance to rotational acceleration

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

            rotation += angularVelocity;
        }

        public abstract void Draw(Graphics g);

        public abstract void DetectEdge(int windowWidth, int windowHeight);

        // "double dispatching"
        // none of these functions are in use, code is pending deletion once it is confirmed they will never be needed
        public abstract bool CollidesWith(Shape other);

        public abstract bool CollidesWithCircle(Circle other);
        public abstract bool CollidesWithRectangle(Rectangle other);

        public abstract PointF[] GetVertices();


        #region Getters & Setters
        public float XAnchor
        {
            get { return xAnchor; }
        }

        public float YAnchor
        {
            get { return yAnchor; }
        }

        public PointF Velocity
        {
            get
            {
                return new PointF(
                    xVelocity,
                    yVelocity
                );
            }
            set
            {
                xVelocity = value.X;
                yVelocity = value.Y;
            }
        }

        public float AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }

        public float Mass
        {
            get { return mass; }
        }

        public float InverseMass
        {
            get { return mass > 0f ? 1.0f / mass : 0f; }
        }

        public float InverseInertia
        {
            get
            {
                return momentOfInertia > 0f
                    ? 1.0f / momentOfInertia
                    : 0f;
            }
        }

        public PointF Position
        {
            get 
            { 
                return new PointF(xAnchor, yAnchor); 
            }
            set
            {
                xAnchor = value.X;
                yAnchor = value.Y;
            }
        }

        public Color Color
        {
            get {  return color; }
            set
            {
                color = value;
            }
        }

        #endregion
    }
}
