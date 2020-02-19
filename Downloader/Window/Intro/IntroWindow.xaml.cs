using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Downloader.View.Intro {
    /// <summary>
    /// Logika interakcji dla klasy IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window {
        DispatcherTimer timer = new DispatcherTimer();
        public IntroWindow() {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0,20);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            progressBar.Value +=1;
            if (progressBar.Value >= 100) {
                timer.Stop();
                progressBar.Value = 0;
                DownloadWindow mainWindow = new DownloadWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
           
        }
    }
}
