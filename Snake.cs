using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using static SnakeGame.Global;

namespace SnakeGame
{
    //public class Snake
    //{
    //    public class Field
    //    {
    //        public int Width, Height;

    //        public List<Cell> Cells;

    //        public Field(int Width, int Height)
    //        {
    //            this.Width = Width;
    //            this.Height = Height;

    //            Cells = new List<Cell>(Width * Height);

    //            for (int x = 0; x < Width; x++)
    //                for (int y = 0; y < Height; y++)
    //                    Cells[y * Width + x] = new Cell(x, y);
    //        }

    //        public void GenFood()
    //        {
    //            List<Cell> EmptyCells = new List<Cell>();
    //            int apples = 0;

    //            foreach (Cell cell in Cells)
    //                switch (cell.type)
    //                {
    //                    case Cell.Type.apple:
    //                        apples++;
    //                        break;
    //                    case Cell.Type.empty:
    //                        EmptyCells.Add(cell);
    //                        break;
    //                    case Cell.Type.snake:
    //                        break;
    //                }
    //        }
    //    }

    //    public class Cell
    //    {
    //        public int X, Y;
    //        public Type type = Type.empty;
    //        public Args args;

    //        public enum Type
    //        { empty, snake, apple }

    //        public class Args
    //        {
    //            public int id, index, count;
    //        }

    //        public Cell(int X, int Y)
    //        {
    //            this.X = X;
    //            this.Y = Y;
    //        }
    //    }
    //}

    public class Field
    {
        public int Width, Height;

        public Cell[] Cells;
        public Snake[] Snakes;

        public Field(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[width * height];
            Snakes = new Snake[] { new Snake() { id = 0, bot = true }, new Snake() { id = 1, bot = true } };

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Cells[y * width + x] = new Cell(x, y, Cell.Type.empty);
        }

        public void GenFood()
        {
            List<Point> aviablePoints = new List<Point>();
            int apples = 0, snakes = 0;
            foreach (Cell cell in Cells)
            {
                switch (cell.type)
                {
                    case Cell.Type.apple:
                        apples++;
                        break;
                    case Cell.Type.empty:
                        aviablePoints.Add(new Point(cell.X, cell.Y));
                        break;
                    case Cell.Type.snake:
                        if (cell.index == 0)
                            snakes++;
                        break;
                }
            }

            Random rnd = new Random();
            while (aviablePoints.Count > 0 && apples <= snakes)
            {
                int val = rnd.Next(aviablePoints.Count);
                Point p = aviablePoints[val];
                aviablePoints.RemoveAt(val);

                apples++;
                Cells[(int)(p.Y * Width + p.X)].type = Cell.Type.apple;
            }
        }

        public void Update()
        {
            foreach (Snake snake in Snakes)
            {
                if (snake.bot)
                    snake.HeadDirection = snake.Bot(this);
                snake.Move(this);
            }
        }

        public class Snake
        {
            public bool alive = true, bot = false;
            public int score = 0, id, length = 1;
            public int?[] debug;
            public Vector HeadDirection
            {
                get
                {
                    if (HeadMoveVectors.Count > 1)
                        HeadMoveVectors.RemoveAt(0);
                    return HeadMoveVectors[0];
                }

                set
                {
                    if (HeadMoveVectors.Count == 0)
                        HeadMoveVectors.Add(value);
                    else
                    {
                        Vector _vector = HeadMoveVectors[^1];
                        if (HeadMoveVectors.Count < 3 &&
                            _vector - value != Vector.Zero &&
                            _vector + value != Vector.Zero)
                            HeadMoveVectors.Add(value);
                    }
                }
            }
            public List<Vector> HeadMoveVectors = new List<Vector>() { Vector.Right };

            public Vector Bot(Field field)
            {
                if (alive)
                {
                    int width = field.Width, height = field.Height;
                    int up, left, down, right;

                    up = left = down = right = int.MaxValue;

                    List<Point> Apples = new List<Point>();
                    Point Head = new Point();

                    int?[] Path = new int?[width * height];

                    foreach (Cell cell in field.Cells)
                    {
                        if (cell.type == Cell.Type.apple)
                            Apples.Add(new Point(cell.X, cell.Y));
                        else if (cell.type == Cell.Type.snake)
                        {
                            Path[cell.Y * width + cell.X] = int.MaxValue;
                            if (cell.id == id && cell.index == 0)
                                Head = new Point(cell.X, cell.Y);
                        }
                    }

                    foreach (Point p in Apples)
                        Path[(int)(p.Y * width + p.X)] = 0;

                    for (int i = 0; i < width * height; i++)
                        for (int j = 0; j < width * height; j++)
                            if (Path[j] == i)
                            {
                                if (j > width)
                                    Path[j - width] ??= i + 1;

                                if (j % width > 0)
                                    Path[j - 1] ??= i + 1;

                                if (j < width * (height - 1))
                                    Path[j + width] ??= i + 1;

                                if (j % width < width - 1)
                                    Path[j + 1] ??= i + 1;
                            }

                    if (DebugOverlay)
                        debug = Path;

                    if (Head.Y > 0)
                        up = Path[(int)(Head.Y * width + Head.X - width)] ?? int.MaxValue >> 1;
                    if (Head.X > 0)
                        left = Path[(int)(Head.Y * width + Head.X - 1)] ?? int.MaxValue >> 1;
                    if (Head.Y < height - 1)
                        down = Path[(int)(Head.Y * width + Head.X + width)] ?? int.MaxValue >> 1;
                    if (Head.X < width - 1)
                        right = Path[(int)(Head.Y * width + Head.X + 1)] ?? int.MaxValue >> 1;

                    int min = new int[4] { up, left, down, right }.Min();

                    if (up == min)
                        return Vector.Up;
                    if (left == min)
                        return Vector.Left;
                    if (down == min)
                        return Vector.Down;
                    if (right == min)
                        return Vector.Right;
                }
                return Vector.Right;
            }

            public void Move(Field field)
            {
                Point Head = new Point();
                foreach (Cell cell in field.Cells) 
                    if (cell.type == Cell.Type.snake)
                        if (cell.id == id && cell.index == 0)
                        {
                            Head = new Point(cell.X, cell.Y);
                            break;
                        }

                Head += HeadDirection;

                alive = Head.X >= 0 && Head.X <= field.Width - 1 &&
                        Head.Y >= 0 && Head.Y <= field.Height - 1 && alive;

                foreach (Cell cell in field.Cells)
                    if (cell.type == Cell.Type.snake)
                        if (cell.id == id)
                            if (cell.index++ > length)
                                cell.type = Cell.Type.empty;

                
                int x = (int)Head.X, y = (int)Head.Y;
                if (alive)
                    switch (field.Cells[y * field.Width + x].type)
                    {
                        case Cell.Type.apple:
                            length++;
                            score++;
                            field.Cells[y * field.Width + x] = new Cell(x, y, id, 0);
                            field.GenFood();
                            break;
                        case Cell.Type.empty:
                            field.Cells[y * field.Width + x] = new Cell(x, y, id, 0);
                            break;
                        case Cell.Type.snake:
                            alive = false;
                            break;
                    }
            }
        }

        public class Cell
        {
            public int X, Y;
            public int? id, index;
            public Type type;

            public enum Type
            {
                apple,
                snake,
                empty
            }

            public Cell(int X, int Y, Type type)
            {
                this.X = X;
                this.Y = Y;
                this.type = type;
            }

            public Cell(int X, int Y, int id, int index)
            {
                this.X = X;
                this.Y = Y;
                this.id = id;
                this.index = index;
                this.type = Type.snake;
            }
        }

        public Grid Draw()
        {
            Grid grid = new Grid() { Width = Width, Height = Height };

            for (int id = 0; id < Snakes.Length; id++)
            {
                List<Cell> TailPoints = new List<Cell>();
                foreach (Cell cell in Cells)
                    switch (cell.type)
                    {
                        case Cell.Type.snake:
                            if (cell.id == id)
                                TailPoints.Add(cell);
                            break;
                        case Cell.Type.apple:
                            Ellipse appleEllipse = new Ellipse()
                            {
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Width = 0.8,
                                Height = 0.8,
                                Fill = appleBrush,
                                Margin = new Thickness(cell.X + 0.1, cell.Y + 0.1, 0, 0)
                            };
                            grid.Children.Add(appleEllipse);
                            break;
                    }

                if (TailPoints.Count > 1)
                    grid.Children.Add(Render(
                        (from cell in TailPoints orderby (int)cell.index
                         select new Point(cell.X, cell.Y)).ToArray(), id));
            }

            Snake[] aliveBots = Snakes.Where(snake => snake.bot && snake.alive).ToArray();

            if (aliveBots.Length > 0)
                if (DebugOverlay && aliveBots[^1].debug != null)
                {
                    int x = 0, y = 0;
                    foreach (int? i in aliveBots[^1].debug)
                    {
                        grid.Children.Add(new Grid()
                        {
                            Margin = new Thickness(x, y, 0, 0),
                            Width = 1,
                            Height = 1,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Children =
                                {
                                    new Ellipse()
                                    {
                                        Width = 0.5,
                                        Height = 0.5,
                                        Fill = new SolidColorBrush(Color.FromArgb(16, 64, 64, 64)),
                                        VerticalAlignment = VerticalAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Center
                                    },
                                    new TextBlock()
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Padding = new Thickness(0),
                                        FontSize = 0.25,
                                        Text = i == int.MaxValue ? "#" : $"{i}",
                                        Foreground = Brushes.White
                                    }
                                }
                        });

                        if (++x >= Width)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

            return grid;
        }

        public Grid Render(Point[] TailPoints, int id)
        {
            Grid grid = new Grid() { Width = Width, Height = Height };
            Point HeadPos = TailPoints[0];
            Brush snakeTailBrush = HsvToRgb(id * 360 / Snakes.Length + 195, 0.85, 1, 1);

            Polyline polyline = new Polyline()
            {
                Stroke = snakeTailBrush,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeThickness = 0.75
            };

            for (int i = Snakes[id].alive ? 1 : 0; i < TailPoints.Length - 1; i++)
                polyline.Points.Add(TailPoints[i] + new Vector(0.5, 0.5));

            if (TailPoints.Length > 2)
            {
                Line start = new Line()
                {
                    X1 = TailPoints[2].X + 0.5,
                    Y1 = TailPoints[2].Y + 0.5,
                    X2 = TailPoints[1].X + 0.5,
                    Y2 = TailPoints[1].Y + 0.5,
                    Stroke = snakeTailBrush,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeThickness = 0.75
                };

                Line end = new Line()
                {
                    X1 = TailPoints[^1].X + 0.5,
                    Y1 = TailPoints[^1].Y + 0.5,
                    X2 = TailPoints[^2].X + 0.5,
                    Y2 = TailPoints[^2].Y + 0.5,
                    Stroke = snakeTailBrush,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeThickness = 0.75
                };

                if (Snakes[id].alive)
                {
                    start.BeginAnimation(Line.X1Property,
                        new DoubleAnimation(start.X2, HeadPos.X + 0.5, GetrefreshTimeSpan()));
                    start.BeginAnimation(Line.Y1Property,
                        new DoubleAnimation(start.Y2, HeadPos.Y + 0.5, GetrefreshTimeSpan()));
                }

                end.BeginAnimation(Line.X1Property,
                    new DoubleAnimation(TailPoints[^1].X + 0.5, end.X2, GetrefreshTimeSpan()));
                end.BeginAnimation(Line.Y1Property,
                    new DoubleAnimation(TailPoints[^1].Y + 0.5, end.Y2, GetrefreshTimeSpan()));

                grid.Children.Add(start);
                grid.Children.Add(end);
            }

            grid.Children.Add(polyline);

            Ellipse headEllipse = new Ellipse()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 0.9,
                Height = 0.9,
                Fill = HsvToRgb(id * 360 / Snakes.Length + 195, 0.3, 1, 1),
                Margin = new Thickness(TailPoints[1].X + 0.05, TailPoints[1].Y + 0.05, 0, 0)
            };

            if (Snakes[id].alive)
                headEllipse.BeginAnimation(FrameworkElement.MarginProperty,
                    new ThicknessAnimation(new Thickness(HeadPos.X + 0.05, HeadPos.Y + 0.05, 0, 0),
                    GetrefreshTimeSpan()));
            else headEllipse.Margin = new Thickness(HeadPos.X + 0.05, HeadPos.Y + 0.05, 0, 0);

            grid.Children.Add(headEllipse);

            return grid;
        }
    }
}

