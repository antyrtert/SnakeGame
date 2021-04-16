using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SnakeGame.Global;

namespace SnakeGame
{
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Debug.IsChecked = DebugOverlay;

            ThemeBox.SelectedIndex = themeId;
        }

        public Theme GetTheme(int id)
        {
            ((userThemesList.Parent as FrameworkElement).Parent as FrameworkElement).Visibility = Visibility.Collapsed;
            switch (id)
            {
                case 0:
                    return new Theme()
                    {
                        appleBrush      = Brushes.OrangeRed,
                        snakeHeadBrush  = Brushes.White,
                        snakeTailBrush  = Brushes.DeepSkyBlue,
                        backgroundBrush = Theme.BrushFromRGBA(51, 204, 51)
                    };

                case 1:
                    return new Theme()
                    {
                        appleBrush      = Theme.BrushFromRGBA(104, 104, 104),
                        snakeTailBrush  = Theme.BrushFromRGBA(51, 51, 51),
                        snakeHeadBrush  = Theme.BrushFromRGBA(64, 64, 64),
                        backgroundBrush = Theme.BrushFromRGBA(34, 34, 34)
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
            List<userTheme> list = Save.Deserialize<userTheme>
                (Environment.CurrentDirectory + "\\Themes.dat");

            int uti = userThemeId;
            userThemesList.Items.Clear();

            foreach (userTheme utheme in list)
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
                                            Fill = theme.backgroundBrush
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
                                            Stroke = theme.snakeTailBrush
                                        },
                                        new Ellipse()
                                        {
                                            Fill = theme.snakeHeadBrush,
                                            Width = 0.9,
                                            Height = 0.9,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            Margin = new Thickness(1.05, 0.55, 0, 0)
                                        },
                                        new Ellipse()
                                        {
                                            Fill = theme.appleBrush,
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
                                Text = utheme.name
                            }
                        }
                    }
                });
            }

            userThemesList.SelectedIndex = uti;
        }

        public class userTheme
        {
            public string name            { get; set; }
            public string appleBrush      { get; set; }
            public string snakeHeadBrush  { get; set; }
            public string snakeTailBrush  { get; set; }
            public string backgroundBrush { get; set; }

            public userTheme() { }
            
            public userTheme(Theme theme) 
            {
                appleBrush      = theme.appleBrush.ToString();
                snakeHeadBrush  = theme.snakeHeadBrush.ToString();
                snakeTailBrush  = theme.snakeTailBrush.ToString();
                backgroundBrush = theme.backgroundBrush.ToString();
            }

            public Theme ToTheme() => new Theme()
            {
                appleBrush      = Theme.BrushFromHex(appleBrush),
                snakeHeadBrush  = Theme.BrushFromHex(snakeHeadBrush),
                snakeTailBrush  = Theme.BrushFromHex(snakeTailBrush),
                backgroundBrush = Theme.BrushFromHex(backgroundBrush)
            };
        }

        public class Theme
        {
            public Brush appleBrush      { get; set; } = Brushes.OrangeRed;
            public Brush snakeHeadBrush  { get; set; } = Brushes.White;
            public Brush snakeTailBrush  { get; set; } = Brushes.DeepSkyBlue;
            public Brush backgroundBrush { get; set; } = BrushFromRGBA(51, 204, 51);

            public Theme() { }

            public void Apply()
            {
                Global.appleBrush      = appleBrush;
                Global.snakeHeadBrush  = snakeHeadBrush;
                Global.snakeTailBrush  = snakeTailBrush;
                Global.backgroundBrush = backgroundBrush;

                ThemeChangedInvoke();
            }

            public static Brush BrushFromHex(string hex) =>
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));

            public static Brush BrushFromRGBA(byte r, byte g, byte b, byte a = 255) =>
                new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private void About(object sender, RoutedEventArgs e)
        {
            var appname = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            new Window
            {
                Owner = this,
                SizeToContent = SizeToContent.WidthAndHeight,
                Style = Application.Current.Resources["WindowStyle"] as Style,
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
                            Content = $"Автор: Воронин Даниил" +
                                $"\nВерсия: {appname.Version}, 2021" +
                                $"\nУправление:" +
                                $"\nWASD или ↑←↓→ - поворот змейки," +
                                $"\nSpace или Enter - начало/перезапуск"
                        },
                        new Rectangle()
                        {
                            Height = 4,
                            Fill = new LinearGradientBrush(
                                Color.FromArgb(48, 0, 0, 0),
                                Color.FromArgb(0, 0, 0, 0), 90)
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
            GetTheme((sender as ComboBox).SelectedIndex).Apply();
        }

        private void UserThemesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            userThemeId = (sender as ComboBox).SelectedIndex;
            (((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Tag as Theme)?.Apply();
        }
    }
}
