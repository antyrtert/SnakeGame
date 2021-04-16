using System.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using static SnakeGame.Global;

namespace SnakeGame
{
    public partial class Scores : Window
    {
        DateTime nextUpdate;
        Timer timer = new Timer(1000);
        public Scores()
        {
            InitializeComponent();

            timer.Elapsed += (_, __) => BlockUpdate();

            List.Loaded += (_, __) => Button_Click_1(null, null);
        }

        private void BlockUpdate()
        {
            if ((nextUpdate - DateTime.Now).TotalSeconds > 0)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ShadeRect.Opacity = 0.5;
                    reload.IsEnabled = false;
                    reload.Content = $"Обновить ({(int)(nextUpdate - DateTime.Now).TotalSeconds + 1})";
                }), null);
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    timer.Stop();
                    ShadeRect.Opacity = 1;
                    reload.IsEnabled = true;
                    reload.Content = "Обновить";
                }), null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            nextUpdate = DateTime.Now + TimeSpan.FromSeconds(5);
            BlockUpdate();
            timer.Start();

            List<item> items = Save.Deserialize<item>(Environment.CurrentDirectory + "\\scores.dat");

            int place = 1;
            foreach (item item in items)
                item.place = $"{place++}";

            items.Insert(0, new item() 
            {
                name = "Имя",
                score = "Счёт",
                time = "Время",
                date = "Дата",
                place = "№" 
            });
            
            List.ItemsSource = items;
        }
    }

    public class user : IComparable<user>
    {
        public string Name { get; set; }
        public List<item> Items { get; set; }
        public int Highscore 
        {
            get
            {
                item item = Items[0];
                for (int i = 1; i < Items.Count; i++)
                    if (item.CompareTo(Items[i]) < 0) 
                        item = Items[i];
                return int.Parse(item.score);
            }
        }

        public int CompareTo([AllowNull] user other) =>
            Highscore.CompareTo(other.Highscore);
    }

    public class item : IComparable<item>
    {
        public string place { get; set; }
        public string name { get; set; }
        public string score { get; set; }
        public string time { get; set; }
        public string date { get; set; }

        public item() { }

        public item(string name, string score, string time, string date)
        {
            this.name = name;
            this.score = score;
            this.time = time;
            this.date = date;
        }

        public int CompareTo([AllowNull] item other)
        {
            int a = int.Parse(score), b = int.Parse(other.score),
                sa = int.Parse(time.Split(':')[0]) * 60 + int.Parse(time.Split(':')[1]),
                sb = int.Parse(other.time.Split(':')[0]) * 60 + int.Parse(other.time.Split(':')[1]);

            return a == b ? (
                    sa == sb ?
                        DateTime.Parse(date).CompareTo(DateTime.Parse(other.date)) 
                        : -sa.CompareTo(sb)) 
                : a.CompareTo(b);
        }
    }
}
