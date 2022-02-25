using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AMath
{
    [Serializable]
    public struct Color
    {
        public double A, R, G, B;

        public byte a => ClampToByte(A);
        public byte r => ClampToByte(R);
        public byte g => ClampToByte(G);
        public byte b => ClampToByte(B);

        public double H => GetHue();
        public double S => Max(R, G, B) == 0 ? 0 : 1d - Min(R, G, B) / Max(R, G, B);
        public double V => Max(R, G, B);
        public double L => R * 0.2627d + G * 0.6780d + B * 0.0593d;

        public static implicit operator System.Windows.Media.Color(Color color) =>
            System.Windows.Media.Color.FromArgb(color.a, color.r, color.g, color.b);

        public static implicit operator Color(System.Windows.Media.Color color) =>
            FromArgb(color.A, color.R, color.G, color.B);

        public static Color FromInt32(uint val)
        {
            byte A = (byte)(val >> 24);
            byte R = (byte)((val & 0xFF0000) >> 16);
            byte G = (byte)((val & 0xFF00) >> 8);
            byte B = (byte)(val & 0xFF);

            return FromArgb(A, R, G, B);
        }

        public static Color FromInt16(ushort val)
        {
            byte A = (byte)(val >> 12);
            byte R = (byte)((val & 0xF00) >> 8);
            byte G = (byte)((val & 0xF0) >> 4);
            byte B = (byte)(val & 0xF);

            A = (byte)((A << 4) + A);
            R = (byte)((R << 4) + R);
            G = (byte)((G << 4) + G);
            B = (byte)((B << 4) + B);

            return FromArgb(A, R, G, B);
        }

        public static Color FromHex(string hex)
        {
            if (new Regex(@"^(#|0x)?([a-fA-F0-9]{8}|[a-fA-F0-9]{6}|[a-fA-F0-9]{4}|[a-fA-F0-9]{3})$").IsMatch(hex))
                return (hex = hex.Replace("#", "0x")).Replace("0x", string.Empty).Length switch
                {
                    3 => FromInt16(unchecked((ushort)(Convert.ToInt16(hex, 16) | 0xF000))),
                    4 => FromInt16(unchecked((ushort)Convert.ToInt16(hex, 16))),
                    6 => FromInt32(unchecked((uint)(Convert.ToInt32(hex, 16) | 0xFF000000))),
                    8 => FromInt32(unchecked((uint)Convert.ToInt32(hex, 16))),

                    _ => new Color()
                };
            return new Color();
        }

        public static Color FromArgb(double A, double R, double G, double B) =>
            new Color() { A = A, R = R, G = G, B = B };

        public static Color FromRgb(double R, double G, double B) =>
            new Color() { A = 1, R = R, G = G, B = B };

        private const double btd = 1d / byte.MaxValue; // Byte to Double
        public static Color FromArgb(byte A, byte R, byte G, byte B) =>
            new Color() { A = A * btd, R = R * btd, G = G * btd, B = B * btd };

        public static Color FromRgb(byte R, byte G, byte B) =>
            new Color() { A = 1, R = R * btd, G = G * btd, B = B * btd };

        public static Color FromHSV(double Hue, double Saturation, double Value, double A = 1)
        {
            Hue %= 360;
            Saturation = Saturation < 1 ? (Saturation > 0 ? Saturation : 0) : 1;
            Value = Value < 1 ? (Value > 0 ? Value : 0) : 1;

            double C = Value * Saturation,
                   H = Hue / 60d,
                   X = C * (1d - Math.Abs(H % 2 - 1)),
                   m = Value - C + 0.5d * btd;

            C += m;
            X += m;

            if (H <= 1) return FromArgb(A, C, X, m);
            else if (H <= 2) return FromArgb(A, X, C, m);
            else if (H <= 3) return FromArgb(A, m, C, X);
            else if (H <= 4) return FromArgb(A, m, X, C);
            else if (H <= 5) return FromArgb(A, X, m, C);
            else if (H <= 6) return FromArgb(A, C, m, X);

            return FromArgb(A, m, m, m);
        }

        public double GetHue()
        {
            double max = Max(R, G, B),
                   min = Min(R, G, B),
                   delta = 60 / (max - min);

            if (max == min) return 0;
            if (max == R) return (G - B) * delta + (G < B ? 360 : 0);
            if (max == G) return (B - R) * delta + 120;
            if (max == B) return (R - G) * delta + 240;

            return 0;
        }

        public override string ToString() => "#" +
            Convert.ToString((a << 24) + (r << 16) + (g << 8) + b, 16).ToUpper().PadLeft(8, '0');

        private static double Max(params double[] value) => value.Max();
        private static double Min(params double[] value) => value.Min();

        private static byte ClampToByte(double i) =>
            (byte)(i > 0 ? (i < 1 ? i * 255 : 255) : 0);
    }
}
