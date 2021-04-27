using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static SnakeGame.Global;
using FocusManager = System.Windows.Input.FocusManager;
using Key = System.Windows.Input.Key;

namespace SnakeGame.Windows
{
    public partial class MainWindow : Window
    {
        private static readonly Vector size = new Vector(11, 11);
        SnakeLogic.Field Field = new SnakeLogic.Field(size.X, size.Y);
        DateTime start;
        readonly System.Timers.Timer updatetimer = new System.Timers.Timer(refreshTime);
        bool MultiPlayer = false;
        int id = 0;

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Width = size.X;
            MainFrame.Height = size.Y;

            Field = GetField();
            Field.Snakes[0].alive = false;

            MainFrame.Children.Add(Field.Draw());

            Closed += (_, __) => updatetimer.Stop();

            updatetimer.Elapsed += (_, __) => Dispatcher.BeginInvoke((Action)(() => Update()), null);
        }

        SnakeLogic.Field GetField()
        {
            switch (PlayModeCB.SelectedIndex)
            {
                case 0:
                    id = 0;
                    MultiPlayer = false;
                    return new SnakeLogic.Field(size.X, size.Y)
                    {
                        Snakes = new List<SnakeLogic.Snake>()
                        {
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 0, bot = false,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 5),
                                    new Point(2, 5),
                                    new Point(1, 5)
                                },
                                HeadPos = new Point(4, 5)
                            }
                        },
                        Apples = new List<Point>()
                        {
                            new Point(7, 5)
                        }
                    };
                case 1:
                    id = 0;
                    MultiPlayer = true;
                    return new SnakeLogic.Field(size.X, size.Y)
                    {
                        Snakes = new List<SnakeLogic.Snake>()
                        {
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 0, bot = false,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 4),
                                    new Point(2, 4),
                                    new Point(1, 4)
                                },
                                HeadPos = new Point(4, 4)
                            },
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 1, bot = true,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 6),
                                    new Point(2, 6),
                                    new Point(1, 6)
                                },
                                HeadPos = new Point(4, 6)
                            }
                        },
                        Apples = new List<Point>()
                        {
                            new Point(7, 4),
                            new Point(7, 6)
                        }
                    };
                case 2:
                    id = 0;
                    MultiPlayer = true;
                    return new SnakeLogic.Field(size.X, size.Y)
                    {
                        Snakes = new List<SnakeLogic.Snake>()
                        {
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 0, bot = true,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 3),
                                    new Point(2, 3),
                                    new Point(1, 3)
                                },
                                HeadPos = new Point(4, 3)
                            },
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 1, bot = true,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 5),
                                    new Point(2, 5),
                                    new Point(1, 5)
                                },
                                HeadPos = new Point(4, 5)
                            },
                            new SnakeLogic.Snake()
                            {
                                Name = username, id = 2, bot = true,
                                TailPoints = new List<Point>()
                                {
                                    new Point(3, 7),
                                    new Point(2, 7),
                                    new Point(1, 7)
                                },
                                HeadPos = new Point(4, 7)
                            }
                        },
                        Apples = new List<Point>()
                        {
                            new Point(7, 3),
                            new Point(7, 5),
                            new Point(7, 7)
                        }
                    };
            }
            return new SnakeLogic.Field(size.X, size.Y);
        }

        public void Start()
        {
            Field = GetField();
            Field.GenFood();

            refreshTime = 1000d / defaultRefreshTime;
            updatetimer.Start();
            start = DateTime.Now;
        }

        private void Update()
        {
            if (MultiPlayer || Field.Snakes[id].alive)
            {
                Field.Update();

                MainFrame.Children.Insert(0, Field.Draw());
                MainFrame.Children.RemoveAt(1);

                string time = $"{(DateTime.Now - start).TotalMinutes:00}:" +
                              $"{(DateTime.Now - start).Seconds:00}";

                if (MultiPlayer &&
                    Field.Snakes.All(snake => snake.bot) &&
                    Field.Snakes.All(snake => !snake.alive))
                    Start();

                NameLabelSP.Children.Clear();
                TimeLabelSP.Children.Clear();
                ScoreLabelSP.Children.Clear();

                foreach (SnakeLogic.Snake snake in Field.Snakes)
                {
                    SolidColorBrush background = HsvToRgb(snake.id * 360 / Field.Snakes.Count + 195, 0.8, 0.8, 0.5);

                    NameLabelSP.Children.Add(new Label()
                    {
                        Padding = new Thickness(5, 3, 5, 3),
                        Content = snake.Name ?? "Без имени",
                        Opacity = snake.alive ? 1 : 0.75,
                        Background = background
                    });

                    ScoreLabelSP.Children.Add(new Label()
                    {
                        Padding = new Thickness(5, 3, 5, 3),
                        Content = snake.score,
                        Opacity = snake.alive ? 1 : 0.75,
                        Background = background
                    });

                    TimeLabelSP.Children.Add(new Label()
                    {
                        Padding = new Thickness(5, 3, 5, 3),
                        Content = snake.time,
                        Opacity = snake.alive ? 1 : 0.75,
                        Background = background
                    });

                    if (snake.alive) snake.time = time;
                }
            }
            else
            {
                updatetimer.Stop();

                if (AskName())
                    SaveResult(new Item(username,
                        $"{Field.Snakes[id].score}",
                        Field.Snakes[id].time,
                        $"{DateTime.Now}"));

                Window wnd = new Window()
                {
                    Owner = this,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Style = Application.Current.Resources["WindowStyle"] as Style,
                    Title = "Игра окончена"
                };

                wnd.PreviewKeyDown += (_, args) =>
                {
                    if (args.Key == Key.Escape)
                        wnd.Close();
                };

                Button rbtn = new Button()
                {
                    Content = "Начать заново",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 3, 0, 0),
                    Focusable = true
                };

                rbtn.Click += (_, __) =>
                {
                    Start();
                    wnd.Close();
                };

                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(15, 15, 15, 12),
                    Children =
                    {
                        new Label()
                        {
                            Padding = new Thickness(5),
                            Content = $"{Field.Snakes[id].Name}\n" +
                            $"Счёт: {Field.Snakes[id].score}, " +
                            $"время: {Field.Snakes[id].time}"
                        },
                        new Rectangle()
                        {
                            Style = Application.Current.Resources["ShadeBox"] as Style
                        },
                        rbtn,
                        new Rectangle()
                        {
                            Style = Application.Current.Resources["ShadeBox"] as Style,
                            Opacity = 1
                        }
                    }
                };

                wnd.Content = sp;
                rbtn.Focus();
                wnd.ShowDialog();
            }
        }

        private bool AskName()
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Window window = new Window()
                {
                    Owner = this,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Style = Application.Current.Resources["WindowStyle"] as Style,
                    Title = "Введите имя"
                };

                TextBox tb = new TextBox()
                {
                    Background = new SolidColorBrush(Color.FromArgb(51, 68, 68, 68)),
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.White,
                    MinWidth = 100,
                    Padding = new Thickness(5),
                    Text = username
                };

                tb.KeyDown += (_, e) =>
                {
                    if (e.Key == Key.Escape || e.Key == Key.Enter)
                        window.Close();
                };


                Button rbtn = new Button() { Content = "ОК" };

                rbtn.Click += (_, __) => window.Close();

                window.Closing += (_, __) =>
                {
                    username = tb.Text;
                    Field.Snakes[id].Name = username;
                };

                window.Content = new StackPanel()
                {
                    Margin = new Thickness(15),
                    Children =
                    {
                        tb,
                        new Rectangle()
                        {
                            Height = 4,
                            Margin = new Thickness(0, 0, 0, 8),
                            Fill = new LinearGradientBrush(
                                Color.FromArgb(48, 0, 0, 0),
                                Color.FromArgb(0, 0, 0, 0), 90)
                        },
                        rbtn,
                        new Rectangle()
                        {
                            Height = 4,
                            Fill = new LinearGradientBrush(
                                Color.FromArgb(96, 0, 0, 0),
                                Color.FromArgb(0, 0, 0, 0), 90)
                        }
                    }
                };

                tb.SelectAll();
                tb.Focus();
                window.ShowDialog();

                return !string.IsNullOrWhiteSpace(username);
            }
            return true;
        }

        private void SaveResult(Item i)
        {
            List<Item> Items = Save.Deserialize<Item>(Environment.CurrentDirectory + "\\Data\\Scores.xml");

            Items.Add(i);
            Items.Sort();
            Items.Reverse();

            if (Items.Count > 1000)
                Items.RemoveRange(1000, Items.Count - 1000);

            Save.Serialize(Items, Environment.CurrentDirectory + "\\Data\\Scores.xml");
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            new Settings() { Owner = this }.ShowDialog();
        private void Button_Click_2(object sender, RoutedEventArgs e) =>
            new Scores() { Owner = this }.ShowDialog();
        private void Button_Click_1(object sender, RoutedEventArgs e) => Start();

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Vector direction = Vector.Zero;
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    direction = Vector.Up;
                    break;
                case Key.A:
                case Key.Left:
                    direction = Vector.Left;
                    break;
                case Key.S:
                case Key.Down:
                    direction = Vector.Down;
                    break;
                case Key.D:
                case Key.Right:
                    direction = Vector.Right;
                    break;
                case Key.Enter:
                case Key.Space:
                    if (!MultiPlayer && !Field.Snakes[id].alive) Start();
                    break;
            }

            if (direction != Vector.Zero && Field.Snakes[id].alive)
                Field.Snakes[id].HeadDirection = direction;

            if (direction != Vector.Zero
                && !Field.Snakes[id].alive
                && !MultiPlayer) Start();
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
            this.Focus();
        }
    }
}
