using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SnakeGame.Global;
using Color = System.Windows.Media.Color;
using Clr = SnakeGame.Global.Color;
using System.Windows.Media.Animation;

namespace SnakeGame.Windows
{
    public partial class PickColor : Window
    {
        bool apply = false;
        public Clr Color;
        public PickColor(Clr clr)
        {
            InitializeComponent();
            Closing += (_, __) => Color = apply ? Color : clr;
            Loaded += (_, __) =>
            {

                HueTB.Text = $"{clr.H:0.000}";
                SaturationTB.Text = $"{clr.S:0.000}";
                ValueTB.Text = $"{clr.V:0.000}";
            };
        }

        private void Update()
        {
            if (HueTB != null && SaturationTB != null && ValueTB != null && HexTB != null)
            {
                if (double.TryParse(HueTB.Text.Replace('.', ','), out double hue))
                    if (double.TryParse(SaturationTB.Text.Replace('.', ','), out double sat))
                        if (double.TryParse(ValueTB.Text.Replace('.', ','), out double val))
                        {
                            Resources["PreviewColorHue"] = (Color)Clr.FromHSV(hue, 1, 1, 1);
                            Resources["PreviewColorSat"] = (Color)Clr.FromHSV(hue, sat, 1, 1);
                            Color = Clr.FromHSV(hue, sat, val, 1);
                            Resources["PreviewColorVal"] = (Color)Color;
                            Resources["PreviewColor"] = (Color)Color;

                            if (Color.L > 0.5)
                                HexTB.Foreground = Brushes.Black;
                            else HexTB.Foreground = Brushes.White;

                            HexTB.Text = Color.ToString().Remove(1, 2);
                        }
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
                double val = Clamp((e.GetPosition(HueGrid).X - 3) / (HueGrid.ActualWidth - 6), 0, 1);

                HueTB.Text = $"{val * 360:0.000}";
            }
        }

        private void SatGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double val = Clamp((e.GetPosition(SatGrid).X - 3) / (SatGrid.ActualWidth - 6), 0, 1);

                SaturationTB.Text = $"{val:0.000}";
            }
        }

        private void ValGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double val = Clamp((e.GetPosition(ValGrid).X - 3) / (ValGrid.ActualWidth - 6), 0, 1);

                ValueTB.Text = $"{val:0.000}";
            }
        }

        private void HueTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count > 0)
                if (HueGrid.ActualWidth > 0)
                    if (double.TryParse(HueTB.Text.Replace('.', ','), out double val))
                        HueCaret.Margin = new Thickness(val / 360d * (HueGrid.ActualWidth - 6), -3, 0, -3);

            Update();
        }

        private void SaturationTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count > 0)
                if (SatGrid.ActualWidth > 0)
                    if (double.TryParse(SaturationTB.Text.Replace('.', ','), out double val))
                        SatCaret.Margin = new Thickness(val * (SatGrid.ActualWidth - 6), -3, 0, -3);

            Update();
        }

        private void ValueTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count > 0)
                if (ValGrid.ActualWidth > 0)
                    if (double.TryParse(ValueTB.Text.Replace('.', ','), out double val))
                        ValCaret.Margin = new Thickness(val * (ValGrid.ActualWidth - 6), -3, 0, -3);

            Update();
        }

        public double Clamp(double value, double min, double max) =>
           value > min ? (value < max ? value : max) : min;

        private void HexTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (!new System.Text.RegularExpressions.Regex(
                @"^#([a-fA-F0-9]{8}|[a-fA-F0-9]{6}|[a-fA-F0-9]{3})$")
                .IsMatch((sender as TextBox).Text))
                return;
            Clr color = Clr.FromHex((sender as TextBox).Text);

            HueTB.Text = $"{color.H:0.000}";
            SaturationTB.Text = $"{color.S:0.000}";
            ValueTB.Text = $"{color.V:0.000}";
        }
    }
}
