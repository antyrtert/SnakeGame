using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SnakeGame.SnakeLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SnakeGame
{
    public static class Global
    {
        public static int themeId = 0;
        public static int userThemeId = 0;
        private static bool _DebugOverlay = false;
        public static bool DebugOverlay
        {
            get => _DebugOverlay;
            set => _DebugOverlay = value;
        }

        public static event Action RedrawField;
        public static void InvokeFieldRedraw() => RedrawField?.Invoke();

        public static object GetResource(string resource) =>
            Application.Current.Resources[resource];

        public static TimeSpan GetrefreshTimeSpan() =>
            TimeSpan.FromMilliseconds(refreshTime);

        public static readonly double defaultRefreshTime = 5;
        public static double refreshTime = 1000 / defaultRefreshTime;
        public static string username;
        public static Field Field;
        public static Field FieldPreset = new Field(11, 11)
        {
            speed = 5,
            Snakes = new List<Snake>()
            {
                new Snake()
                {
                    Name = username, id = 0, bot = false,
                    Color = Color.FromHSV(195, 1, 1, 1),
                    TailPoints = new List<Point>()
                    {
                        new Point(3, 5),
                        new Point(2, 5),
                        new Point(1, 5),
                        new Point(0, 5)
                    },
                    HeadPos = new Point(4, 5)
                }
            },
            Apples = new List<Point>()
            {
                new Point(5, 5)
            }
        };

        public static T GetResource<T>(string name) =>
            (T)Application.Current.Resources[name];

        public static T Clone<T>(object obj) =>
            JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));

        public static class Save
        {
            public static void SerializeBin<T>(IList<T> o, string file)
            {
                MemoryStream ms = new MemoryStream();
                using (BsonWriter writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, o);
                }

                File.WriteAllText(file, Convert.ToBase64String(ms.ToArray()));
            }

            public static IList<T> DeSerializeBin<T>(string file)
            {
                byte[] data = Convert.FromBase64String(File.ReadAllText(file));

                MemoryStream ms = new MemoryStream(data);
                using (BsonReader reader = new BsonReader(ms))
                {
                    reader.ReadRootValueAsArray = true;
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<IList<T>>(reader);
                }
            }
        }

        public class Color
        {
            public double A, R, G, B;

            public byte a => ClampToByte(A);
            public byte r => ClampToByte(R);
            public byte g => ClampToByte(G);
            public byte b => ClampToByte(B);

            public double H => ((System.Drawing.Color)this).GetHue();
            public double S => Max(R, G, B) == 0 ? 0 : 1d - Min(R, G, B) / Max(R, G, B);
            public double V => Max(R, G, B);
            public double L => R * 0.3 + G * 0.59 + B * 0.11;

            public static implicit operator System.Drawing.Color(Color color) =>
                System.Drawing.Color.FromArgb(color.a, color.r, color.g, color.b);

            public static implicit operator System.Windows.Media.Color(Color color) =>
                System.Windows.Media.Color.FromArgb(color.a, color.r, color.g, color.b);

            public static implicit operator Color(System.Windows.Media.Color color) =>
                FromArgb(color.A, color.R, color.G, color.B);

            public static implicit operator Color(System.Drawing.Color color) =>
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
                if (new System.Text.RegularExpressions.Regex(
                    @"^#([a-fA-F0-9]{8}|[a-fA-F0-9]{6}|[a-fA-F0-9]{4}|[a-fA-F0-9]{3})$").IsMatch(hex))
                    return (System.Windows.Media.Color)ColorConverter.ConvertFromString(hex);
                else return new Color();
            }

            public static Color FromArgb(double A, double R, double G, double B) =>
                new Color() { A = A, R = R, G = G, B = B };

            public static Color FromRgb(double R, double G, double B) =>
                new Color() { A = 1, R = R, G = G, B = B };

            public static Color FromArgb(byte A, byte R, byte G, byte B) =>
                new Color() { A = A / 255d, R = R / 255d, G = G / 255d, B = B / 255d };

            public static Color FromRgb(byte R, byte G, byte B) =>
                new Color() { A = 1, R = R / 255d, G = G / 255d, B = B / 255d };

            public static Color FromHSV(double H, double S, double V, double a = 255)
            {
                while (H < 0) H += 360;
                while (H >= 360) H -= 360;

                int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
                double f = H / 60 - Math.Floor(H / 60);

                double v = V;
                double p = V * (1 - S);
                double q = V * (1 - f * S);
                double t = V * (1 - (1 - f) * S);

                if (hi == 0) return FromArgb(a, v, t, p);
                else if (hi == 1) return FromArgb(a, q, v, p);
                else if (hi == 2) return FromArgb(a, p, v, t);
                else if (hi == 3) return FromArgb(a, p, q, v);
                else if (hi == 4) return FromArgb(a, t, p, v);
                else return FromArgb(a, v, p, q);
            }

            public override string ToString() => "#" +
                Convert.ToString((a << 24) + (r << 16) + (g << 8) + b, 16).ToUpper().PadLeft(8, '0');

            private static double Max(double a, double b, double c) =>
                a > b ? (a > c ? a : c) : (b > c ? b : c);

            private static double Min(double a, double b, double c) =>
                a < b ? (a < c ? a : c) : (b < c ? b : c);

            private static byte ClampToByte(double i) =>
                (byte)(i > 0 ? (i < 1 ? i * 255 : 255) : 0);
        }
    }
}
