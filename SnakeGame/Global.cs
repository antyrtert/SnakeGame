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

        public static object GetResource(string resource) =>
            Application.Current.Resources[resource];

        public static TimeSpan GetrefreshTimeSpan() =>
            TimeSpan.FromMilliseconds(refreshTime);

        public static readonly double defaultRefreshTime = 5;
        public static double refreshTime = 1000 / defaultRefreshTime;
        public static string username;

        public static class Save
        {
            public static void Serialize<T>(List<T> list, string file)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                string xml;
                try
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(stringWriter, list);
                        xml = stringWriter.ToString();
                    }
                    File.WriteAllText(file, xml, Encoding.Default);
                }
                catch (Exception args)
                {
                    new Window()
                    {
                        Title = args.ToString(),
                        Content = args.Message
                    };
                };
            }

            public static List<T> Deserialize<T>(string file)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                List<T> list = new List<T>();
                try
                {
                    using StreamReader sr = new StreamReader(file);
                    list = (List<T>)xmlSerializer.Deserialize(sr);
                }
                catch (Exception args)
                {
                    new Window()
                    {
                        Title = args.ToString(),
                        Content = args.Message
                    };
                };
                return list;
            }
        }

        public static SolidColorBrush HsvToRgb(double H, double S, double V, double a)
        {
            while (H < 0) H += 360;
            while (H >= 360) H -= 360;

            int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
            double f = H / 60 - Math.Floor(H / 60);

            V *= 255;
            int v = Convert.ToInt32(V);
            int p = Convert.ToInt32(V * (1 - S));
            int q = Convert.ToInt32(V * (1 - f * S));
            int t = Convert.ToInt32(V * (1 - (1 - f) * S));

            if (hi == 0)
                return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(v),
                Clamp(t),
                Clamp(p)));
            else if (hi == 1)
                return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(q),
                Clamp(v),
                Clamp(p)));
            else if (hi == 2)
                return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(p),
                Clamp(v),
                Clamp(t)));
            else if (hi == 3)
                return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(p),
                Clamp(q),
                Clamp(v)));
            else if (hi == 4)
                return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(t),
                Clamp(p),
                Clamp(v)));
            else return new SolidColorBrush(Color.FromArgb(
                Clamp((int)(a * 255)),
                Clamp(v),
                Clamp(p),
                Clamp(q)));
        }

        public static byte Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return (byte)i;
        }
    }
}
