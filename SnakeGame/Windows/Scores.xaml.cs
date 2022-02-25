using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using static SnakeGame.Global;

namespace SnakeGame.Windows
{
    public partial class Scores : Window
    {
        DateTime nextUpDate;
        readonly Timer Timer = new Timer(1000);
        public Scores()
        {
            InitializeComponent();

            Timer.Elapsed += (_, __) => BlockUpdate();

            List.Loaded += (_, __) => Button_Click_1(null, null);
        }

        private void BlockUpdate()
        {
            if ((nextUpDate - DateTime.Now).TotalSeconds > 0)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ShadeRect.Opacity = 0.5;
                    reload.IsEnabled = false;
                    reload.Content = $"Обновить ({(int)(nextUpDate - DateTime.Now).TotalSeconds + 1})";
                }), null);
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Timer.Stop();
                    ShadeRect.Opacity = 1;
                    reload.IsEnabled = true;
                    reload.Content = "Обновить";
                }), null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            nextUpDate = DateTime.Now + TimeSpan.FromSeconds(5);
            BlockUpdate();
            Timer.Start();

            List<Item> Items = (List<Item>)Save.DeSerializeBin<Item>(Environment.CurrentDirectory + "\\Data\\Scores.bin");

            int Place = 1;
            foreach (Item Item in Items)
                Item.Place = $"{Place++}";

            Items.Insert(0, new Item()
            {
                Name = "Имя",
                Score = "Счёт",
                Time = "Время",
                Date = "Дата",
                Place = "№"
            });

            List.ItemsSource = Items;
        }
    }

    [Serializable]
    public class Item : IComparable<Item>
    {
        public string Place { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }

        public Item() { }

        public Item(string Name, string Score, string Time, string Date)
        {
            this.Name = Name;
            this.Score = Score;
            this.Time = Time;
            this.Date = Date;
        }

        public int CompareTo(Item other)
        {
            int a = int.Parse(Score), b = int.Parse(other.Score),
                sa = int.Parse(Time.Split(':')[0]) * 60 + int.Parse(Time.Split(':')[1]),
                sb = int.Parse(other.Time.Split(':')[0]) * 60 + int.Parse(other.Time.Split(':')[1]);

            return a == b ? (sa == sb ? DateTime.Parse(Date).CompareTo(DateTime.Parse(other.Date)) : -sa.CompareTo(sb)) : a.CompareTo(b);
        }
    }
}
