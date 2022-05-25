using System.Windows;
using System.Windows.Controls;

namespace Word_Learning.MVVM.View
{
    public partial class LearningView : UserControl
    {
        public LearningView()
        {
            InitializeComponent();
        }

        private void ButtonDownload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DownloadWindow window = new DownloadWindow();
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
