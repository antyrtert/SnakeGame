using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnakeGame
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Window window = new Window()
            {
                Title = e.Exception.Message,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33)),
                Width = 500,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            Button close = new Button()
            {
                Margin = new Thickness(8, 8, 4, 8),
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(0x88, 0xFF, 0x33, 0x33)),
                Content = "Close"
            };

            Button ignore = new Button()
            {
                Margin = new Thickness(4, 8, 8, 8),
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(0x88, 0x33, 0xFF, 0x33)),
                Content = "Continue"
            };

            close.Click += (_, __) =>
            {
                e.Handled = false;
                window.Close();

                foreach (Window wnd in Windows)
                    wnd.Close();

                Shutdown();
            };

            ignore.Click += (_, __) =>
            {
                e.Handled = true;
                window.Close();
            };

            Grid.SetColumn(close, 0);
            Grid.SetColumn(ignore, 1);

            Grid grid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(0x22, 0, 0, 0)),
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition()
                },
                Children =
                {
                    close,
                    ignore
                }
            };

            Grid.SetRow(grid, 1);

            Grid stackPanel = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition()
                    {
                        Height = new GridLength(1, GridUnitType.Auto)
                    }
                },
                Children =
                {
                    new ScrollViewer()
                    {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = new TextBox()
                        {
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            IsReadOnly = true,
                            Foreground = Brushes.White,
                            Text = e.Exception.ToString()
                        }
                    },
                    grid
                }
            };

            window.Content = stackPanel;

            window.ShowDialog();
        }
    }
}
