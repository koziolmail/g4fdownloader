using System.Windows;

namespace Downloader {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window {
        public DownloadWindow() {
            DataContext = new DownloadWindowModel(
                new Download.DownloadService(),
                new Download.ChooseDirectoryService(),
                new Download.LinkService());
            InitializeComponent();
        }
    }
}
