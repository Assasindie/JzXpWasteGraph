using Discord.Webhook;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection TotalXpCollection { get; set; }
        public SeriesCollection XPDiffCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


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

            Labels = FileHandler.Date.ToArray();
            YFormatter = value => value.ToString();
            DataContext = this;
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
            DiscordWebhookClient discord = new DiscordWebhookClient(1, "token");
            discord.SendFileAsync(Environment.CurrentDirectory + @"\" + ChartName + ".png", "");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }
    }
}
