using System.Windows;
using System.Windows.Input;
using Word_Learning.MVVM.ViewModel;

namespace Word_Learning.MVVM.View
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(Window owner, object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public static void GoodDialog(Window owner, string message = "")
        {
            new MessageWindow(owner, MessageViewModel.Good(message)).ShowDialog();
        }

        public static void BadDialog(Window owner, string message = "")
        {
            new MessageWindow(owner, MessageViewModel.Bad(message)).ShowDialog();
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
