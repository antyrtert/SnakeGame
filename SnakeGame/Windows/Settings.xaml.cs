using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SnakeGame.Global;
using Color = System.Windows.Media.Color;

namespace SnakeGame.Windows
{
    public partial class Settings : Window
    {
        public List<SnakeLogic.Snake> Snakes;
        public Settings()
        {
            InitializeComponent();

            ThemeBox.SelectedIndex = themeId;
            Debug.IsChecked = DebugOverlay;

            switch (FieldPreset.Width)
            {
                case 9:
                    FieldSizeCB.SelectedIndex = 0;
                    break;
                case 11:
                    FieldSizeCB.SelectedIndex = 1;
                    break;
                case 13:
                    FieldSizeCB.SelectedIndex = 2;
                    break;
                case 15:
                    FieldSizeCB.SelectedIndex = 3;
                    break;
                case 17:
                    FieldSizeCB.SelectedIndex = 4;
                    break;
                case 21:
                    FieldSizeCB.SelectedIndex = 5;
                    break;
            }
            Snakes = Clone<List<SnakeLogic.Snake>>(FieldPreset.Snakes);

            SnakesLV.ItemsSource = Snakes;
        }

        public Theme GetTheme(int id)
        {
            ((userThemesList.Parent as FrameworkElement).Parent as FrameworkElement).Visibility = Visibility.Collapsed;
            switch (id)
            {
                case 0:
                    return new Theme()
                    {
                        Apple = Colors.OrangeRed,
                        Background = Color.FromArgb(0xFF, 0x33, 0xCC, 0x33)
                    };

                case 1:
                    return new Theme()
                    {
                        Apple = Color.FromArgb(0xFF, 0x66, 0x66, 0x66),
                        Background = Color.FromArgb(0xFF, 0x33, 0x33, 0x33)
                    };

                case 2:
                    ((userThemesList.Parent as FrameworkElement).Parent as FrameworkElement).Visibility = Visibility.Visible;
                    LoadUserThemes();
                    return (userThemesList?.SelectedItem as ListBoxItem)?.Tag as Theme ?? new Theme();

                default:
                    break;
            }
            return new Theme();
        }

        public void LoadUserThemes()
        {
            List<UserTheme> list =
                JsonConvert.DeserializeObject<List<UserTheme>>(
                    File.ReadAllText(Environment.CurrentDirectory + "\\Data\\Themes.json"));

            int uti = userThemeId;
            userThemesList.Items.Clear();

            foreach (UserTheme utheme in list)
            {
                Theme theme = utheme.ToTheme();
                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(@"/checker.bmp", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.None,
                    Opacity = 0.02,
                    Width = 2,
                    Height = 2
                };
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                userThemesList.Items.Add(new ComboBoxItem()
                {
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0),
                    Tag = theme,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new Viewbox()
                            {
                                Margin = new Thickness(3, 1, 3, 1),
                                Width = 40,
                                Child = new Grid()
                                {
                                    Children = {
                                        new Rectangle()
                                        {
                                            Width = 4,
                                            Fill = new SolidColorBrush(theme.Background)
                                        },
                                        img,
                                        new Line()
                                        {
                                            X1 = 0.5,
                                            X2 = 1.5,
                                            Y1 = 1,
                                            Y2 = 1,
                                            StrokeEndLineCap = PenLineCap.Round,
                                            StrokeStartLineCap = PenLineCap.Round,
                                            StrokeThickness = 0.75,
                                            Stroke = GetResource("AccentBrush") as Brush
                                        },
                                        new Ellipse()
                                        {
                                            Fill = Brushes.White,
                                            Width = 0.9,
                                            Height = 0.9,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            Margin = new Thickness(1.05, 0.55, 0, 0)
                                        },
                                        new Ellipse()
                                        {
                                            Fill = new SolidColorBrush(theme.Apple),
                                            Width = 0.8,
                                            Height = 0.8,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            Margin = new Thickness(3.1, 0.6, 0, 0)
                                        }
                                    }
                                }
                            },
                            new TextBlock()
                            {
                                Padding = new Thickness(5, 3, 5, 3),
                                Text = utheme.Name
                            }
                        }
                    }
                });
            }

            userThemesList.SelectedIndex = uti;
        }

        public class UserTheme
        {
            public string Name { get; set; }
            public string Apple { get; set; }
            public string Background { get; set; }

            public UserTheme() { }

            public UserTheme(Theme theme)
            {
                Apple = theme.Apple.ToString();
                Background = theme.Background.ToString();
            }

            public Theme ToTheme() => new Theme()
            {
                Apple = Theme.ColorFromHex(Apple),
                Background = Theme.ColorFromHex(Background)
            };
        }

        public class Theme
        {
            public Color Apple { get; set; } = Colors.OrangeRed;
            public Color Background { get; set; } = Color.FromArgb(255, 51, 204, 51);

            public Theme() { }

            public void Apply()
            {
                Application.Current.Resources["Apple"]      = Apple;
                Application.Current.Resources["Background"] = Background;

                byte v = (byte)((((Global.Color)Background).V > 1 / 5f) ? 0x22 : 0x66);

                Application.Current.Resources["BaseHigh"]   = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                Application.Current.Resources["AltHigh"]    = Color.FromArgb(0x88, v, v, v);
                Application.Current.Resources["AltMedHigh"] = Color.FromArgb(0x66, v, v, v);
                Application.Current.Resources["AltMed"]     = Color.FromArgb(0x44, v, v, v);
                Application.Current.Resources["AltMedLow"]  = Color.FromArgb(0x33, v, v, v);
                Application.Current.Resources["AltLow"]     = Color.FromArgb(0x22, v, v, v);
            }

            public static Color ColorFromHex(string hex) =>
                (Color)ColorConverter.ConvertFromString(hex);
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            string s = (FieldSizeCB.SelectedItem as ComboBoxItem).Content as string;
            int w = int.Parse(s.Split('x')[0]), h = int.Parse(s.Split('x')[1]);

            if (Snakes.Count == 0)
                Snakes.Add(new SnakeLogic.Snake()
                {
                    Color = Global.Color.FromInt32(0xFF00BEFF)
                });

            for (int i = 0; i < Snakes.Count; i++)
            {
                int y = h * (i + 1) / Snakes.Count - h / (Snakes.Count + 1);
                Snakes[i].id = i;
                Snakes[i].TailPoints = new List<Point>()
                {
                    new Point(3, y),
                    new Point(2, y),
                    new Point(1, y),
                    new Point(0, y)
                };
                Snakes[i].HeadPos = new Point(4, y);
            }

            FieldPreset = new SnakeLogic.Field(w, h)
            {
                Snakes = SnakesLV.ItemsSource as List<SnakeLogic.Snake>,
                maxApples = Snakes.Count,
                Apples = new List<Point>()
                {
                    new Point(w / 2, h / 2)
                }
            };

            if (Field.Snakes.TrueForAll(snake => !snake.alive))
                InvokeFieldRedraw();
        }

        private void AddSnakeBtnClick(object sender, RoutedEventArgs e)
        {
            if (Snakes.Count < 5)
                Snakes.Add(new SnakeLogic.Snake()
                {
                    Color = Global.Color.FromHSV(new Random(Snakes.Count * Snakes.Count).NextDouble() * 360, 1, 1, 1),
                    bot = true
                });

            SnakesLV.ItemsSource = new List<SnakeLogic.Snake>();
            SnakesLV.ItemsSource = Snakes;
        }

        private void RemoveSnakeBtnClick(object sender, RoutedEventArgs e)
        {
            if (SnakesLV.SelectedIndex >= 0)
                Snakes.RemoveAt(SnakesLV.SelectedIndex);

            if (Snakes.Count == 0)
                Snakes.Add(new SnakeLogic.Snake()
                {
                    Color = Global.Color.FromInt32(0xFF00BEFF)
                });

            SnakesLV.ItemsSource = new List<SnakeLogic.Snake>();
            SnakesLV.ItemsSource = Snakes;
        }

        private void About(object sender, RoutedEventArgs e)
        {
            var appname = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            new Window
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                Style = GetResource<Style>("WindowStyle"),
                ResizeMode = ResizeMode.NoResize,
                Title = "О игре",
                Content = new StackPanel()
                {
                    Margin = new Thickness(15),
                    Children =
                    {
                        new Label()
                        {
                            Padding = new Thickness(5),
                            Content =
                                $"{appname.Name}, версия: {appname.Version}, 2021" +
                                $"\nАвтор: Воронин Даниил" +
                                $"\n\nУправление:" +
                                $"\nWASD или ↑←↓→ - поворот змейки," +
                                $"\nSpace или Enter - начало/перезапуск"
                        },
                        new Rectangle()
                        {
                            Style = GetResource<Style>("ShadeBox")
                        }
                    }
                },
            }.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) =>
            DebugOverlay = true;
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e) =>
            DebugOverlay = false;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            themeId = (sender as ComboBox).SelectedIndex;
            GetTheme(themeId).Apply();
        }

        private void UserThemesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            userThemeId = (sender as ComboBox).SelectedIndex;
            (((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Tag as Theme)?.Apply();
        }

        private void UsernameTB_TextChanged(object sender, TextChangedEventArgs e) =>
            username = (sender as TextBox).Text;

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PickColor wnd = new PickColor(((sender as Rectangle).Fill as SolidColorBrush).Color)
            {
                Owner = this
            };

            wnd.ShowDialog();
            (sender as Rectangle).Fill = new SolidColorBrush(wnd.Color);

            SnakesLV.ItemsSource = new List<SnakeLogic.Snake>();
            SnakesLV.ItemsSource = Snakes;
        }
    }
}
