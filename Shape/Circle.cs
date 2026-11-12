
namespace WindowsGUI
{
    public class Circle : Shape
    {
        private float radius;

        public Circle (float x, float y, float r) : base(x, y) 
        {
            radius = r;

        }

        public void Draw(Graphic)
        {
            Console.WriteLine("Draw function was called from the Circle class... Nothing happened.");
        }
    }

}
