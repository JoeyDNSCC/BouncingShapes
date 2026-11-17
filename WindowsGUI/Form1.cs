using NAudio.Wave;
using System.Drawing;
using System.Windows.Forms;
using WindowsGUI;
using static WindowsGUI.CollisionDetector;

namespace WindowsGUI
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        List<Shape> shapes = new List<Shape>();
        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ResizeRedraw = true;

            myTimer.Interval = 30;
            myTimer.Tick += MyTimer_Tick;

            myTimer.Start();
        }

        private void MyTimer_Tick(object? sender, EventArgs e)
        {
            // Move all shapes
            foreach (Shape shape in shapes)
            {
                shape.Move();

                shape.DetectEdge(
                    ClientSize.Width,
                    ClientSize.Height
                );
            }

            // Check collisions
            for (int i = 0; i < shapes.Count; i++)
            {

                for (int j = i + 1; j < shapes.Count; j++)
                {
                    Shape a = shapes[i];
                    Shape b = shapes[j];

                    CollisionInfo collision;

                    if (CollisionDetector.SATCollision(a, b, out collision))
                    {
                        CollisionDetector.ResolveCollision(a, b, collision);

                    }
                }
            }

            // Redraw
            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //shapes.Add(new Circle(ClientSize.Width / 2, ClientSize.Height / 2, 80));
        }

        private void ResizeWindow(object sender, EventArgs e)
        {
            
        }


        private void DrawFrame(object sender, PaintEventArgs e)
        {
            foreach (Shape shape in shapes)
            {
                shape.Draw(e.Graphics);
            }

        }
        private void Form1_Click(object sender, EventArgs e)
        {
            //Point coords = System.Windows.Forms.Control.MousePosition;
            var relativePoint = this.PointToClient(Cursor.Position);
            AddShape(relativePoint.X, relativePoint.Y);
        }

        private void AddShape(int x, int y)
        {
            Random random = new Random();
            int shapeType = random.Next(1, 3);
            //int shapeType = 1;
            int height = random.Next(30, 100);
            int width = random.Next(30, 100);

            switch (shapeType)
            {
                case 0:
                    shapes.Add(new Circle(x, y, width)); break;
                case 1:
                    shapes.Add(new Rectangle(x, y, height, width)); break;
                case 2:
                    shapes.Add(new Polygon(x, y)); break;
                case 3:
                    shapes.Add(new Cube(x, y, width)); break;
            }
        }
    }
}
