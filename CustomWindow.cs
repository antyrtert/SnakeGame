using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnakeGame
{
    public class DefaultWindow : Window
    {
        Grid RootGrid, MainGrid;
        Border Host;
        Label label;
        Button MinimizeButton, MaximizeButton;
        new Image Icon;

        private void InitializeComponent()
        {
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = System.Windows.WindowStyle.None;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Width = 350;
            Height = 200;
            MinWidth = 204;
            MinHeight = 98;

            MainGrid = new Grid() 
            {
                Background = Global.backgroundBrush,
                Margin = new Thickness(-1) 
            };

            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            label = new Label();
            label.MouseDoubleClick += Maximize_Window;
            MainGrid.Children.Add(label);

            MinimizeButton = new Button() 
            { 
                Width = 45,
                Background = Brushes.Transparent,
                Content = new Border()
                {
                    SnapsToDevicePixels = true,
                    Height = 10,
                    Width = 12,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(0, 0, 0, 3)
                }
            };
            MinimizeButton.GotFocus += Button_GotFocus;
            MinimizeButton.Click += Minimize_Window;

            MaximizeButton = new Button() 
            { 
                Width = 45,
                Background = Brushes.Transparent,
                Content = new Border()
                {
                    SnapsToDevicePixels = true,
                    Height = 10, Width = 12,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2, 3, 2, 2)
                }
            };
            MaximizeButton.GotFocus += Button_GotFocus;
            MaximizeButton.Click += Maximize_Window;

            Button CloseButton = new Button()
            {
                Width = 45,
                Background = Brushes.Transparent,
                Content = new Path()
                {
                    SnapsToDevicePixels = false,
                    StrokeThickness = 3,
                    Stroke = Brushes.White,
                    Data = Geometry.Parse("M 0,0 L 12,12 M 0,12 L 12,0") 
                }
            };
            CloseButton.GotFocus += Button_GotFocus;
            CloseButton.Click += Close_Window;

            MainGrid.Children.Add(new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal,
                Height = 25,
                Children = { MinimizeButton, MaximizeButton, CloseButton }
            });

            Icon = new Image()
            {
                IsHitTestVisible = false,
                Margin = new Thickness(2),
                Height = 21,
                Width = 21,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Source = new BitmapImage(
                         new Uri("pack://application:,,,/icon.ico", UriKind.Absolute))
            };

            RenderOptions.SetBitmapScalingMode(Icon, BitmapScalingMode.Fant);

            MainGrid.Children.Add(Icon);

            MainGrid.Children.Add(new Rectangle()
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, -3, 0, -3),
                Height = 3,
                Fill = new LinearGradientBrush(
                    Color.FromArgb(34, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0), 90)
            });

            RootGrid = new Grid();
            Grid.SetRow(RootGrid, 1);
            MainGrid.Children.Add(RootGrid);

            Host = new Border()
            {
                BorderBrush = new SolidColorBrush(Color.FromArgb(34, 0, 0, 0)),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(12),
                Child = MainGrid
            };

            Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                BlurRadius = 7,
                Direction = 270,
                Opacity = 0.5,
                ShadowDepth = 0.5,
                Color = Color.FromRgb(51, 51, 51)
            };

            Content = Host;
        }

        public DefaultWindow()
        {
            InitializeComponent();

            CanResize = true;

            PreviewMouseMove += window_PreviewMouseMove;

            Global.ThemeChanged += () => MainGrid.Background = Global.backgroundBrush;

            this.PreviewKeyDown += window_PreviewKeyDown;
            this.PreviewKeyUp += window_PreviewKeyDown;

            this.MouseLeftButtonDown += (_, __) =>
            {
                if (WindowState == WindowState.Normal)
                {
                    Point p = __.GetPosition(this);

                    double max = Host.Margin.Left + 2, min = Host.Margin.Left - 3;
                    Vector v = new Vector(0, 0);

                    if (p.X <= max && p.X > min)
                        v.X = -1;
                    else if (p.X >= ActualWidth - max && p.X < ActualWidth - min)
                        v.X = 1;

                    if (p.Y <= max && p.Y > min)
                        v.Y = -1;
                    else if (p.Y >= ActualHeight - max && p.Y < ActualHeight - min)
                        v.Y = 1;

                    ResizeDirection = v;

                    LeftButtonDown = true;
                    Mouse.Capture(this);
                }
            };

            this.MouseLeftButtonUp += (_, __) =>
            {
                LeftButtonDown = false;
                this.ReleaseMouseCapture();
            };
        }

        private async void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.PrintScreen && e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                bool succes = false;
                await Dispatcher.BeginInvoke((Action)(() => ScreenShot(out succes)), null);

                e.Handled = succes;
            }
        }

        public void ScreenShot(out bool succes)
        {
            int t = 10;
            while (t-- > 0)
            {
                try
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                        (int)Host.ActualWidth, (int)Host.ActualHeight,
                        96, 96, PixelFormats.Pbgra32);

                    renderTargetBitmap.Render(MainGrid);

                    Clipboard.SetImage(renderTargetBitmap);

                    GC.Collect();
                    succes = true;

                    return;
                }
                catch (Exception) {  }

                GC.Collect();
                System.Threading.Thread.Sleep(200);
            }
            succes = false;
        }

        public string Header
        {
            get => $"{label.Content}";
            set => label.Content = value;
        }

        private Point DownPoint;
        private bool LeftButtonDown = false;
        private Vector ResizeDirection = new Vector(0, 0);
        public bool IconVisible
        {
            get => Icon.Visibility == Visibility.Visible;
            set => Icon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public bool CanResize { get; set; }
        public bool CanMinimize
        {
            get => MinimizeButton.Visibility == Visibility.Visible;
            set => MinimizeButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public bool CanMaximize
        {
            get => MaximizeButton.Visibility == Visibility.Visible;
            set => MaximizeButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public UIElement MainContent
        {
            get => RootGrid.Children[0];
            set
            {
                RootGrid.Children.Clear();
                RootGrid.Children.Add(value);
            }
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
            this.Focus();
        }

        private void Close_Window(object sender, RoutedEventArgs args) => Close();
        private void Move_Window(Point p)
        {
            if (WindowState == WindowState.Maximized)
            {
                double width = ActualWidth;

                WindowState = WindowState.Normal;
                Host.Margin = new Thickness(12);

                Left = p.X - p.X / width * ActualWidth - 12;
                Top = p.Y * 0.5;
            }
            DragMove();
        }
        private void Minimize_Window(object sender, RoutedEventArgs args) =>
            WindowState = ResizeMode != ResizeMode.NoResize ? WindowState.Minimized : WindowState;
        private void Maximize_Window(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            Host.Margin = new Thickness(WindowState == WindowState.Maximized ? -1 : 12);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private void window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (CanResize && WindowState == WindowState.Normal)
            {
                Point p = e.GetPosition(this);
                Vector v = new Vector(0, 0);

                double max = Host.Margin.Left + 2, min = Host.Margin.Left - 3;
                if (p.X <= max && p.X > min)
                    v.X = -1;
                else if (p.X >= ActualWidth - max && p.X < ActualWidth - min)
                    v.X = 1;

                if (p.Y <= max && p.Y > min)
                    v.Y = -1;
                else if (p.Y >= ActualHeight - max && p.Y < ActualHeight - min)
                    v.Y = 1;

                if (!LeftButtonDown)
                if (v.X == 1 && v.Y == -1 || v.X == -1 && v.Y == 1)
                    this.Cursor = Cursors.SizeNESW;
                else if (v.X == 1 && v.Y == 1 || v.X == -1 && v.Y == -1)
                    this.Cursor = Cursors.SizeNWSE;
                else if (v.X != 0 && v.Y == 0)
                    this.Cursor = Cursors.SizeWE;
                else if (v.X == 0 && v.Y != 0)
                    this.Cursor = Cursors.SizeNS;
                else this.Cursor = Cursors.Arrow;

                if (LeftButtonDown && ResizeDirection != Vector.Zero)
                {
                    double t = Top, l = Left, w = Width, h = Height;
                    if (ResizeDirection.X == 1)
                    {
                        double Offset = p.X - ActualWidth + Host.Margin.Left;
                        if (Offset + Width < 0)
                            w = MinWidth;
                        else w = Offset + Width;
                    }

                    if (ResizeDirection.Y == 1)
                    {
                        double Offset = p.Y - ActualHeight + Host.Margin.Top;
                        if (Offset + Height < 0)
                            h = MinHeight;
                        else h = Offset + Height;
                    }

                    if (ResizeDirection.X == -1)
                    {
                        double Right = this.Left + ActualWidth;
                        double Left = this.Left + p.X - Host.Margin.Left;

                        if (Right - Left >= MinWidth)
                            l = Left;
                        else l = Right - MinWidth;
                        w = Right - Left;
                    }

                    if (ResizeDirection.Y == -1)
                    {
                        double Bottom = this.Top + ActualHeight;
                        double Top = this.Top + p.Y - Host.Margin.Top;

                        if (Bottom - Top >= MinHeight)
                            t = Top;
                        else t = Bottom - MinHeight;
                        h = Bottom - Top;
                    }

                    Top = t;
                    Left = l;
                    Width = w > 0 ? w : MinWidth;
                    Height = h > 0 ? h : MinHeight;
                    e.Handled = true;
                }
            }

            if (!e.Handled && e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(label);
                if (p.X >= 0 && p.X <= label.ActualWidth &&
                    p.Y >= 0 && p.Y <= label.ActualHeight)
                    DownPoint ??= p;

                if (!ReferenceEquals(p, null) && !ReferenceEquals(DownPoint, null))
                    if (Math.Abs(p.X - DownPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(p.Y - DownPoint.Y) >= SystemParameters.MinimumVerticalDragDistance)
                        Move_Window(p);
            }
            else DownPoint = null;
        }
    }
}
