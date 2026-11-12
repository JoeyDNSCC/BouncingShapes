using System.Windows.Forms;
using WindowsGUI;
using System.Drawing;

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
            foreach (Shape shape in shapes)
            {
                shape.Move();
                shape.DetectEdge(ClientSize.Width, ClientSize.Height);
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shape != shapes[i])
                        shape.CollidesWith(shapes[i]);
                }
            }
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
            //int shapeType = random.Next(0, 2);
            int shapeType = 1;
            int height = random.Next(20, 100);
            int width = random.Next(20, 100);

            switch (shapeType)
            {
                case 0:
                    shapes.Add(new Circle(x, y, width)); break;
                case 1:
                    shapes.Add(new Rectangle(x, y, height, width)); break;
            }
        }
    }
}
