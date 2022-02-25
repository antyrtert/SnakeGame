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
using Vector = AMath.Vector;

namespace SnakeGame.Windows
{
    public partial class MainWindow : Window
    {
        DateTime StartTime;
        readonly System.Timers.Timer updatetimer = new System.Timers.Timer(refreshTime);
        bool MultiPlayer => Field.Snakes.Count > 1;
        public int id = 0;

        public MainWindow()
        {
            InitializeComponent();

            Field = FieldPreset.Clone();
            Field.Snakes[0].alive = false;
            MainFrame.Children.Add(Field.Draw());

            RedrawField += () =>
            {
                MainFrame.Width = FieldPreset.Width;
                MainFrame.Height = FieldPreset.Height;
                MainFrame.Children.Insert(0, FieldPreset.Draw());
                MainFrame.Children.RemoveAt(1);
            };

            Closed += (_, __) => updatetimer.Stop();

            updatetimer.Elapsed += (_, __) => Dispatcher.BeginInvoke((Action)Update, null);
        }

        public void Start()
        {
            Field = FieldPreset.Clone();
            Field.GenFood();

            MainFrame.Width = Field.Width;
            MainFrame.Height = Field.Height;

            refreshTime = 1000d / Field.speed;
            updatetimer.Interval = refreshTime;
            updatetimer.Start();
            StartTime = DateTime.Now;
        }

        private void Update()
        {
            Field.Update();

            string time = (DateTime.Now - StartTime).TotalMinutes.ToString("00") +
                    ':' + (DateTime.Now - StartTime).Seconds.ToString("00");

            NameLabelSP.Children.Clear();
            TimeLabelSP.Children.Clear();
            ScoreLabelSP.Children.Clear();

            List<SnakeLogic.Snake> Snakes = Field.Snakes.ToArray().ToList();
            Snakes.Sort((p, n) => p.score.CompareTo(n.score));
            Snakes.Sort((p, n) => p.alive.CompareTo(n.alive));
            Snakes.Reverse();

            foreach (SnakeLogic.Snake snake in Snakes)
            {
                SolidColorBrush background = new(snake.Color);
                SolidColorBrush foreground = Brushes.White;

                background.Opacity = 0.5;

                NameLabelSP.Children.Add(new Label()
                {
                    Padding = new Thickness(5, 3, 5, 3),
                    Content = snake.Name ?? "Без имени",
                    Opacity = snake.alive ? 1 : 0.75,
                    Foreground = foreground,
                    Background = background
                });

                ScoreLabelSP.Children.Add(new Label()
                {
                    Padding = new Thickness(5, 3, 5, 3),
                    Content = snake.score,
                    Opacity = snake.alive ? 1 : 0.75,
                    Foreground = foreground,
                    Background = background
                });

                TimeLabelSP.Children.Add(new Label()
                {
                    Padding = new Thickness(5, 3, 5, 3),
                    Content = snake.time,
                    Opacity = snake.alive ? 1 : 0.75,
                    Foreground = foreground,
                    Background = background
                });

                if (snake.alive) snake.time = time;
            }

            if (MultiPlayer || Field.Snakes[id].alive)
            {
                MainFrame.Children.Insert(0, Field.Draw());
                MainFrame.Children.RemoveAt(1);

                if (MultiPlayer &&
                    Field.Snakes.All(snake => snake.bot) &&
                    Field.Snakes.All(snake => !snake.alive))
                    Start();
            }
            else
            {
                updatetimer.Stop();

                if (Field.Width == 11 && Field.Height == 11
                    && Field.Snakes.Count == 1 && Field.Snakes[0].bot == false
                    && Field.maxApples == 1 && Field.speed == 5)
                    SaveResult(new Item(username, $"{Field.Snakes[id].score}", Field.Snakes[id].time, $"{DateTime.Now}"));

                Window wnd = new()
                {
                    Owner = this,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Style = GetResource<Style>("WindowStyle"),
                    Title = "Игра окончена"
                };

                wnd.PreviewKeyUp += (_, args) =>
                {
                    if (args.Key == Key.Escape)
                        wnd.Close();
                };

                Button rbtn = new()
                {
                    Content = "Начать заново",
                    Focusable = true
                };

                rbtn.Click += (_, __) =>
                {
                    Start();
                    wnd.Close();
                };

                StackPanel sp = new()
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
                            Style = GetResource<Style>("ShadeBox")
                        },
                        rbtn,
                        new Rectangle()
                        {
                            Style = GetResource<Style>("ShadeBox"),
                            Opacity = 1
                        }
                    }
                };

                wnd.Content = sp;
                rbtn.Focus();
                wnd.ShowDialog();
            }

        }

        private void SaveResult(Item i)
        {
            List<Item> Items = Save.DeSerializeBin<Item>(Environment.CurrentDirectory + "\\Data\\Scores.bin");

            Items.Add(i);
            Items.Sort();
            Items.Reverse();

            if (Items.Count > 100)
                Items.RemoveRange(100, Items.Count - 100);

            Save.SerializeBin(Items, Environment.CurrentDirectory + "\\Data\\Scores.bin");
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            new Settings() { Owner = this }.ShowDialog();
        private void Button_Click_1(object sender, RoutedEventArgs e) =>
            Start();
        private void Button_Click_2(object sender, RoutedEventArgs e) =>
            new Scores() { Owner = this }.ShowDialog();

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
                    direction = Vector.Right;
                    break;
            }

            if (Field.Snakes.All(snake => !snake.alive)
                && direction != Vector.Zero
                && !MultiPlayer) Start();

            if (direction != Vector.Zero && Field.Snakes[id].alive)
                Field.Snakes[id].HeadDirection = direction;
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
            this.Focus();
        }
    }
}
