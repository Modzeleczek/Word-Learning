using System;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private Window window;
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

        public RelayCommand WindowLoaded { get; }
        public RelayCommand Login { get; private set; }
        public RelayCommand Register { get; private set; }

        public event EventHandler OnRequestClose;

        public LoginViewModel()
        {
            WindowLoaded = new RelayCommand(e => window = (Window)e);
            Register = new RelayCommand(e =>
            {
                if (string.IsNullOrWhiteSpace(Username))
                { Error("Specify a username."); return; }
                if (!ValidatePassword()) return;
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

        private bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            { Error("Specify a password."); return false; }
            if (Password.Length < 8)
            { Error("Password should be at least 8 characters long."); return false; }
            bool hasDigit = false;
            foreach (var c in Password)
                if (c >= '0' && c <= '9') { hasDigit = true; break; }
            if (!hasDigit) { Error("Password should contain at least one digit."); return false; }
            bool hasSpecial = false;
            foreach (var c in Password)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                { hasSpecial = true; break; }
            if (!hasSpecial)
            {
                Error("Password should contain at least one special character (not a letter or a digit).");
                return false;
            }
            return true;
        }
    }
}
