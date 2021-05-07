using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SnakeGame.SnakeLogic;
using System;
using System.Collections.Generic;
using System.IO;
using AMath;
using Point = AMath.Point;

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
            System.Windows.Application.Current.Resources[resource];

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
            (T)System.Windows.Application.Current.Resources[name];

        public static void SetResource<T>(string name, T value) =>
            System.Windows.Application.Current.Resources[name] = value;

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
    }
}
