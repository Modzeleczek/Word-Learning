using System;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        public Window window;
        private void Error(string message)
            => new MessageWindow(window, MessageViewModel.Bad(message)).ShowDialog();
        private void Success(string message)
            => new MessageWindow(window, MessageViewModel.Good(message)).ShowDialog();

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(nameof(Password)); }
        }

        public RelayCommand Login { get; }
        public RelayCommand Register { get; }

        public event EventHandler OnRequestClose;

        public LoginViewModel()
        {
            
            Register = new RelayCommand(e =>
            {
                if (string.IsNullOrWhiteSpace(Username))
                { Error("Specify a username."); return; }
                if (string.IsNullOrWhiteSpace(Password))
                { Error("Specify a password."); return; }
                var status = Storage.AddUser(new User { Username = Username, Password = Password });
                if (status.Code == 0) Success("Please log in to your new account.");
                else Error(status.Message);
            });
            Login = new RelayCommand(e =>
            {
                var status = Storage.GetUser(Username, Password, out User user);
                if (status.Code == 0)
                {
                    User.Instance = user;
                    OnRequestClose(this, new EventArgs());
                }
                else Error(status.Message);
            });
        }
    }
}
