using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGUI
{
    public class Triangle : Shape
    {
        protected int height, width;
        
        public Triangle(float x, float y, int h, int w) : base(x, y)
        {
            height = h;
            width = w;
        }

        public override void Draw(Graphics g)
        {
            throw new NotImplementedException();
        }
        public override void DetectEdge(int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
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
