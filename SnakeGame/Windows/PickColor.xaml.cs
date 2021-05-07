using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace SnakeGame.Windows
{
    public partial class PickColor : Window
    {
        Regex hex = new Regex(@"^#([a-fA-F0-9]{6})$");
        bool apply = false;
        private double H, S, V;
        public AMath.Color Color;
        public PickColor(AMath.Color clr)
        {
            InitializeComponent();
            Closing += (_, __) => Color = apply ? Color : clr;

            (H, S, V) = (clr.H, clr.S, clr.V);

            Loaded += (_, __) => Update();
        }

        private void Update()
        {
            Resources["PreviewColorHue"] = (Color)AMath.Color.FromHSV(H, 1, 1, 1);
            Resources["PreviewColorSat"] = (Color)AMath.Color.FromHSV(H, S, 1, 1);
            Resources["PreviewColorVal"] = (Color)AMath.Color.FromHSV(H, S, V, 1);
            Resources["PreviewColor"] = (Color)AMath.Color.FromHSV(H, S, V, 1);

            Color = AMath.Color.FromHSV(H, S, V);

            if (HexTB is TextBox && HueTB is TextBox && SaturationTB is TextBox && ValueTB is TextBox)
            {
                if (HueCaret is Rectangle && SatCaret is Rectangle && ValCaret is Rectangle)
                {
                    HueCaret.Margin = new Thickness(H / 360d * (HueGrid.ActualWidth - 6), -3, 0, -3);
                    SatCaret.Margin = new Thickness(S * (SatGrid.ActualWidth - 6), -3, 0, -3);
                    ValCaret.Margin = new Thickness(V * (ValGrid.ActualWidth - 6), -3, 0, -3);
                }

                int hexTBcaret = HexTB.CaretIndex,
                    hueTBcaret = HueTB.CaretIndex,
                    saturationTBcaret = SaturationTB.CaretIndex,
                    valueTBcaret = ValueTB.CaretIndex;

                HueTB.Text = $"{H:0.000}";
                SaturationTB.Text = $"{S:0.000}";
                ValueTB.Text = $"{V:0.000}";

                HexTB.Text = Color.ToString().Remove(1, 2);

                HexTB.CaretIndex = hexTBcaret;
                HueTB.CaretIndex = hueTBcaret;
                SaturationTB.CaretIndex = saturationTBcaret;
                ValueTB.CaretIndex = valueTBcaret;

                HexTB.Foreground = Color.L > 0.5 ? Brushes.Black : Brushes.White;
            }
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            apply = true;
            Close();
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e) =>
            (sender as Grid).CaptureMouse();

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e) =>
            (sender as Grid).ReleaseMouseCapture();

        private void HueGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double val = Clamp((e.GetPosition(HueGrid).X - 3) / (HueGrid.ActualWidth - 6));

                H = val * 360;
                Update();
            }
        }

        private void SatGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double val = Clamp((e.GetPosition(SatGrid).X - 3) / (SatGrid.ActualWidth - 6));

                S = val;
                Update();
            }
        }

        private void ValGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double val = Clamp((e.GetPosition(ValGrid).X - 3) / (ValGrid.ActualWidth - 6));

                V = val;
                Update();
            }
        }

        private void HueTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(HueTB.Text.Replace('.', ','), out double val))
                H = System.Math.Abs(val % 360);

            Update();
        }

        private void SaturationTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(SaturationTB.Text.Replace('.', ','), out double val))
                S = Clamp(val);

            Update();
        }

        private void ValueTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(ValueTB.Text.Replace('.', ','), out double val))
                V = Clamp(val);

            Update();
        }

        public double Clamp(double value, double min = 0, double max = 1) =>
           value > min ? (value < max ? value : max) : min;

        private void HexTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (!hex.IsMatch((sender as TextBox).Text))
                return;

            AMath.Color color = AMath.Color.FromHex((sender as TextBox).Text);

            H = color.H;
            S = color.S;
            V = color.V;

            Update();
        }
    }
}
