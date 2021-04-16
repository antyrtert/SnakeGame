using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static SnakeGame.Global;
using FocusManager = System.Windows.Input.FocusManager;
using Key = System.Windows.Input.Key;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private static readonly Vector size = new Vector(11, 11);
        Field field = new Field((int)size.X, (int)size.Y);
        DateTime start;
        System.Timers.Timer updatetimer = new System.Timers.Timer(refreshTime);
        bool MultiPlayer = true;
        int id = 0;

        public MainWindow()
        {
            InitializeComponent();
            
            MainFrame.Width = size.X;
            MainFrame.Height = size.Y;

            foreach (Field.Snake snake in field.Snakes)
            {
                snake.alive = false;
                for (int i = 1; i < 4; i++)
                    field.Cells[(int)(size.Y / field.Snakes.Length * snake.id + size.Y / field.Snakes.Length * 0.5) + i] = 
                        new Field.Cell(i, (int)(size.Y / field.Snakes.Length * snake.id + size.Y / field.Snakes.Length * 0.5), snake.id, 3 - i);
            }

            MainFrame.Children.Add(field.Draw());

            ThemeChanged += () =>
            {
                MainFrame.Children.Insert(0, field.Draw());
                MainFrame.Children.RemoveAt(1);
            };

            Closed += (_, __) => updatetimer.Stop();

            updatetimer.Elapsed += (_, __) => Dispatcher.BeginInvoke((Action)(() => Update()), null);
        }

        public void Start()
        {
            field = new Field((int)size.X, (int)size.Y);

            foreach (Field.Snake snake in field.Snakes)
                for (int i = 2; i < 4; i++)
                    field.Cells[(int)(size.Y / field.Snakes.Length * snake.id + size.Y / field.Snakes.Length * 0.5) + i] =
                        new Field.Cell(i, (int)(size.Y / field.Snakes.Length * snake.id + size.Y / field.Snakes.Length * 0.5), snake.id, 3 - i);

            field.GenFood();

            refreshTime = 1000 / defaultRefreshTime;
            updatetimer.Start();
            start = DateTime.Now;
        }

        private void Update()
        {
            if (MultiPlayer || field.Snakes[id].alive)
            {
                TimeLabel.Content = $"{(DateTime.Now - start).TotalMinutes:00}:" +
                    $"{(DateTime.Now - start).Seconds:00}";

                if (MultiPlayer)
                {
                    if (field.Snakes.All(snake => !snake.alive))
                        Start();
                }
                else
                {
                    ScoreLabel.Content = field.Snakes[id].score;
                }

                field.Update();
                MainFrame.Children.Insert(0, field.Draw());
                MainFrame.Children.RemoveAt(1);
            }
            else 
            {
                updatetimer.Stop();

                if (AskName()) SaveResult(new item(username, field.Snakes[id].score.ToString(),
                    TimeLabel.Content.ToString(), DateTime.Now.ToString()));

                Window wnd = new Window()
                {
                    Owner = this,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
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
                    Margin = new Thickness(0, 3, 0, 0)
                };

                rbtn.Click += (_, __) =>
                {
                    Start();
                    wnd.Close();
                };

                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(15),
                    Children =
                    {
                        new Label()
                        {
                            Padding = new Thickness(5),
                            Content = $"{username}\n" +
                            $"Счёт: {field.Snakes[id].score}, время: {TimeLabel.Content}"
                        },
                        new Rectangle()
                        {
                            Height = 4,
                            Fill = new LinearGradientBrush(
                                Color.FromArgb(51, 0, 0, 0),
                                Color.FromArgb(0, 0, 0, 0), 90)
                        },
                        rbtn,
                        new Rectangle()
                        {
                            Height = 4,
                            Fill = new LinearGradientBrush(
                                Color.FromArgb(102, 0, 0, 0),
                                Color.FromArgb(0, 0, 0, 0), 90)
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
            if (username == "Без имени" || string.IsNullOrWhiteSpace(username))
            {
                Window window = new Window()
                {
                    Owner = this,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
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
                    if (e.Key == Key.Escape || e.Key == Key.Enter && !string.IsNullOrWhiteSpace(tb.Text))
                        window.Close();
                };


                Button rbtn = new Button() { Content = "ОК" };

                rbtn.Click += (_, __) => window.Close();

                window.Closing += (_, __) =>
                {
                    username = tb.Text;
                    UsernameLabel.Content = tb.Text;
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

                return username != "Без имени" && !string.IsNullOrWhiteSpace(username);
            }
            return true;
        }

        private void SaveResult(item i)
        {
            List<item> items = Save.Deserialize<item>(Environment.CurrentDirectory + "\\scores.dat");

            items.Add(i);
            items.Sort();
            items.Reverse();

            if (items.Count > 1000)
                items.RemoveRange(1000, items.Count - 1000);

            Save.Serialize(items, Environment.CurrentDirectory + "\\scores.dat");
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
                    if (!MultiPlayer && !field.Snakes[0].alive) Start();
                    break;
            }

            if (field.Snakes[id].alive && direction != Vector.Zero)
                field.Snakes[id].HeadDirection = direction;
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
            this.Focus();
        }
    }
}
