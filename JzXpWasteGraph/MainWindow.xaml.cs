using Discord.Webhook;
using JzXpWasteGraph;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace GraphTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection TotalXpCollection { get; set; }
        public SeriesCollection XPDiffCollection { get; set; }
        public SeriesCollection SkillsCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        private readonly DiscordWebhookClient discord = new DiscordWebhookClient(1, "token");

        public MainWindow()
        {
            InitializeComponent();
            FileHandler.LoadFile();
            TotalXpCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "JZ's XP",
                    Values = new ChartValues<long>(FileHandler.XP),
                    LineSmoothness = 0
                },
            };

            XPDiffCollection = new SeriesCollection
            {
                new ColumnSeries {
                    Title = "JZ's Xp Difference",
                    Values = new ChartValues<long>(FileHandler.XPDiff)
                },
            };

            SkillsCollection = new SeriesCollection();
            //create a List of Lists of skills
            Type playerinfo = typeof(PlayerInfo);
            foreach (PropertyInfo prop in playerinfo.GetProperties().Where(prop => prop.PropertyType == typeof(Skill) && prop.Name != "Overall"))
            {
                SkillsCollection.Add(new LineSeries
                {
                    Title = prop.Name,
                    Values = new ChartValues<long>(FileHandler.GetSkillList(FileHandler.PlayerInfos, prop.Name, "Experience")),
                    LineSmoothness = 0,
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }

                DateTime[] DateValues = FileHandler.Date.ToArray();
            string[] DateStringValues = new string[DateValues.Length];
            for (int i = 0; i < DateValues.Length; i++)
            {
                DateStringValues[i] = DateValues[i].ToString("MMM d");
            }
            Labels = DateStringValues;
            YFormatter = value => value.ToString();
            DataContext = this;
            DiffChart.DataTooltip.Background = Brushes.Black;
            DiffChart.DataTooltip.Foreground = Brushes.White;
            SkillsChart.DataTooltip.Background = Brushes.Black;
            SkillsChart.DataTooltip.Foreground = Brushes.White;
            TotalChart.DataTooltip.Background = Brushes.Black;
            TotalChart.DataTooltip.Foreground = Brushes.White;
        }

        private void SaveFile()
        {
            CartesianChart Chart = TotalXPHeader.IsSelected ? TotalChart : DiffChart;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Chart.ActualWidth, (int)Chart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(Chart);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream stream = new MemoryStream();
            png.Save(stream);
            Image image = Image.FromStream(stream);
            stream.Dispose();
            string ChartName = TotalXPHeader.IsSelected ? "TotalChart" : "DiffChart";
            image.Save(ChartName + ".png");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ChartName = TotalXPHeader.IsSelected ? "TotalChart" : "DiffChart";
            SaveFile();
            discord.SendFileAsync(Environment.CurrentDirectory + @"\" + ChartName + ".png", "");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            string str = "Total XP : ";
            foreach (long value in FileHandler.XP)
            {
                str += " " + value;
            }

            str += " XP Diff : ";

            foreach (long value in FileHandler.XPDiff)
            {
                str += " " + value;
            }

            str += " Dates : ";

            foreach (DateTime value in FileHandler.Date)
            {
                str += $" \"{value.Day}/{value.Month}\"";
            }

            Clipboard.SetText(str);
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            string stats = $" Total Days of Data : {FileHandler.XPDiff.Count}" +
                $" \n Total Days with 0 XP gain : {FileHandler.TotalDaysWasted}" +
                $" \n Longest Streak without XP gain : {FileHandler.LongestStreak} " +
                $" \n Most XP Gained in 1 Day : {FileHandler.XPDiff.Max()}" +
                $" \n Average XP Gained per Day : {FileHandler.XPDiff.Average()}";
            MessageBox.Show(stats);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            discord.Dispose();
        }
    }
}
