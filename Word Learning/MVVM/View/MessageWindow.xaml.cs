using System.Windows;
using System.Windows.Input;
using Word_Learning.MVVM.ViewModel;

namespace Word_Learning.MVVM.View
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            var app = Application.Current;
            try { Owner = app.MainWindow; }
            // jest wyrzucany, jeżeli MainWindow jeszcze się nie pojawiło
            catch (System.InvalidOperationException)
            {
                if (app.Windows.Count > 0 && app.Windows[1] != this) // MainWindow ma indeks 0
                    Owner = Application.Current.Windows[1];
            }
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public static void GoodDialog(string message = "")
        {
            new MessageWindow(MessageViewModel.Good(message)).ShowDialog();
        }

        public static void BadDialog(string message = "")
        {
            new MessageWindow(MessageViewModel.Bad(message)).ShowDialog();
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
