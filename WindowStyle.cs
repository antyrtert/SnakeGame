using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SnakeGame
{
    internal static class Extensions
    {
        public static void WindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
        {
            Window window = ((FrameworkElement)templateFrameworkElement).TemplatedParent as Window;
            if (window != null) action(window);
        }
    }

    partial class WindowStyle
    {
        private void Loaded(object sender, RoutedEventArgs args)
        {
           (sender as Window).Background = Global.backgroundBrush;

            Global.ThemeChanged += () =>
                (sender as Window).Background = Global.backgroundBrush;

            (sender as Window).PreviewKeyDown += window_PreviewKeyDown;
            (sender as Window).PreviewKeyUp += window_PreviewKeyDown;
            ((HwndSource)PresentationSource.FromVisual(sender as Window)).AddHook(HookProc);
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.PrintScreen && e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                try
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                        (int)(sender as Window).ActualWidth,
                        (int)(sender as Window).ActualHeight,
                        96, 96, System.Windows.Media.PixelFormats.Pbgra32);

                    renderTargetBitmap.Render(sender as Window);

                    CroppedBitmap crop = new CroppedBitmap(renderTargetBitmap,
                        new Int32Rect(8, 8,
                        (int)(sender as Window).ActualWidth - 16,
                        (int)(sender as Window).ActualHeight - 16));

                    Clipboard.SetImage(crop);
                }
                catch (Exception) { }

                GC.Collect();
                e.Handled = true;
            }
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

        private void CloseButtonClicked(object sender, RoutedEventArgs e) =>
            sender.WindowFromTemplate(w => w.Close());
        
        private void MaximizeButtonClicked(object sender, RoutedEventArgs e) =>
            sender.WindowFromTemplate(w => ToggleState(w, WindowState.Maximized));

        private void MinimizeButtonClicked(object sender, RoutedEventArgs e) =>
            sender.WindowFromTemplate(w => ToggleState(w, WindowState.Minimized));

        private WindowState ToggleState(Window window, WindowState state) =>
            window.WindowState == state ?
            window.WindowState = WindowState.Normal :
            window.WindowState = state;

    }
}
