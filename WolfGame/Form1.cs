using System;
using System.Drawing;
using System.Windows.Forms;

namespace WolfGame
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Timer countdownTimer;
        private Wolf playerWolf;
        private Item item;
        private Wall[] walls;
        private Random random;
        private int score;
        private int timeLeft;

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 600);

            playerWolf = new Wolf("Alpha", 30, new Point(400, 300));
            random = new Random();
            item = new Item(new Point(random.Next(ClientSize.Width), random.Next(ClientSize.Height)));
            score = 0;
            timeLeft = 30; // 30-second countdown

            walls = new Wall[]
            {
                new Wall(new Rectangle(100, 100, 600, 20)),
                new Wall(new Rectangle(100, 200, 20, 300)),
                new Wall(new Rectangle(300, 300, 400, 20)),
                new Wall(new Rectangle(500, 400, 20, 100))
            };

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 100; // Update every 100 ms
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; // Update every 1 second
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            this.Paint += MainForm_Paint;
            this.KeyDown += MainForm_KeyDown;
            this.KeyUp += MainForm_KeyUp;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            playerWolf.UpdatePosition();
            CheckCollision();
            Invalidate(); // Triggers the Paint event
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
            }
            else
            {
                gameTimer.Stop();
                countdownTimer.Stop();
                MessageBox.Show($"Time's up! Your score is {score}", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.CornflowerBlue);
            playerWolf.Draw(g);
            item.Draw(g);

            foreach (var wall in walls)
            {
                wall.Draw(g);
            }

            // Draw score and time left
            g.DrawString($"Score: {score}", new Font("Arial", 16), Brushes.White, new PointF(10, 10));
            g.DrawString($"Time Left: {timeLeft} s", new Font("Arial", 16), Brushes.White, new PointF(10, 40));
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    playerWolf.SetVelocity(-10, 0);
                    break;
                case Keys.D:
                    playerWolf.SetVelocity(10, 0);
                    break;
                case Keys.W:
                    playerWolf.SetVelocity(0, -10);
                    break;
                case Keys.S:
                    playerWolf.SetVelocity(0, 10);
                    break;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.D:
                case Keys.W:
                case Keys.S:
                    playerWolf.SetVelocity(0, 0);
                    break;
            }
        }

        private void CheckCollision()
        {
            Rectangle wolfRect = new Rectangle(playerWolf.Position.X, playerWolf.Position.Y, 50, 50);
            Rectangle itemRect = new Rectangle(item.Position.X, item.Position.Y, 10, 10);

            if (wolfRect.IntersectsWith(itemRect))
            {
                score++;
                item = new Item(new Point(random.Next(ClientSize.Width), random.Next(ClientSize.Height)));
            }

            foreach (var wall in walls)
            {
                if (wolfRect.IntersectsWith(wall.Rectangle))
                {
                    // Reposition wolf to avoid collision
                    playerWolf.SetVelocity(0, 0);
                    if (playerWolf.Position.X < wall.Rectangle.Left)
                    {
                        playerWolf.Position = new Point(wall.Rectangle.Left - 50, playerWolf.Position.Y);
                    }
                    else if (playerWolf.Position.X + 50 > wall.Rectangle.Right)
                    {
                        playerWolf.Position = new Point(wall.Rectangle.Right, playerWolf.Position.Y);
                    }
                    else if (playerWolf.Position.Y < wall.Rectangle.Top)
                    {
                        playerWolf.Position = new Point(playerWolf.Position.X, wall.Rectangle.Top - 50);
                    }
                    else if (playerWolf.Position.Y + 50 > wall.Rectangle.Bottom)
                    {
                        playerWolf.Position = new Point(playerWolf.Position.X, wall.Rectangle.Bottom);
                    }
                }
            }
        }
    }
}
    public class Wolf
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Point Position { get; set; }
        private Point velocity;

        public Wolf(string name, int age, Point startPosition)
        {
            Name = name;
            Age = age;
            Position = startPosition;
        }

        public void Draw(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 2);
            Brush grayBrush = new SolidBrush(Color.Gray);
            Brush darkGrayBrush = new SolidBrush(Color.DarkGray);

            // Head
            g.FillEllipse(grayBrush, Position.X, Position.Y, 50, 50);
            g.DrawEllipse(pen, Position.X, Position.Y, 50, 50);

            // Ears
            PointF[] leftEar = { new PointF(Position.X + 10, Position.Y), new PointF(Position.X + 20, Position.Y - 20), new PointF(Position.X + 30, Position.Y) };
            g.FillPolygon(grayBrush, leftEar);
            g.DrawPolygon(pen, leftEar);
            PointF[] rightEar = { new PointF(Position.X + 20, Position.Y), new PointF(Position.X + 30, Position.Y - 20), new PointF(Position.X + 40, Position.Y) };
            g.FillPolygon(grayBrush, rightEar);
            g.DrawPolygon(pen, rightEar);

            // Eyes
            g.FillEllipse(Brushes.White, Position.X + 15, Position.Y + 20, 8, 8);
            g.DrawEllipse(pen, Position.X + 15, Position.Y + 20, 8, 8);
            g.FillEllipse(Brushes.White, Position.X + 27, Position.Y + 20, 8, 8);
            g.DrawEllipse(pen, Position.X + 27, Position.Y + 20, 8, 8);

            // Pupils
            g.FillEllipse(Brushes.Black, Position.X + 18, Position.Y + 23, 3, 3);
            g.FillEllipse(Brushes.Black, Position.X + 30, Position.Y + 23, 3, 3);

            // Nose
            g.FillEllipse(Brushes.Black, Position.X + 22, Position.Y + 35, 6, 6);

            // Fur texture
            for (int i = 0; i < 5; i++)
            {
                g.DrawArc(pen, Position.X + 10 + i * 5, Position.Y + 45, 10, 10, 0, 180);
            }
        }

        public void Move(int dx, int dy)
        {
            Position = new Point(Position.X + dx, Position.Y + dy);
        }

        public void SetVelocity(int dx, int dy)
        {
            velocity = new Point(dx, dy);
        }

        public void UpdatePosition()
        {
            Position = new Point(Position.X + velocity.X, Position.Y + velocity.Y);
        }
    }

    // Continue with the Item and Wall classes...
    namespace WolfGame
    {
        public class Item
        {
            public Point Position { get; set; }

            public Item(Point position)
            {
                Position = position;
            }

            public void Draw(Graphics g)
            {
                Brush brush = new SolidBrush(Color.Gold);
                g.FillEllipse(brush, Position.X, Position.Y, 10, 10);
            }
        }

        public class Wall
        {
            public Rectangle Rectangle { get; set; }

            public Wall(Rectangle rectangle)
            {
                Rectangle = rectangle;
            }

            public void Draw(Graphics g)
            {
                Brush brush = new SolidBrush(Color.Brown);
                g.FillRectangle(brush, Rectangle);
            }
        }
    }