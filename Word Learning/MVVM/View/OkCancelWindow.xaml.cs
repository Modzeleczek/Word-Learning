using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Word_Learning.MVVM.ViewModel;

namespace Word_Learning.MVVM.View
{
    /// <summary>
    /// Interaction logic for OkCancelWindow.xaml
    /// </summary>
    public partial class OkCancelWindow : Window
    {
        public OkCancelWindow(Window owner, object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public static void Dialog(Window owner, string message = "")
        {
            new MessageWindow(owner, new OkCancelViewModel { MessageText = message }).ShowDialog();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
