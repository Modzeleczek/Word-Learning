using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(nameof(Message)); }
        }
        private Brush messageBrush;
        public Brush MessageBrush
        {
            get { return messageBrush; }
            set { messageBrush = value; OnPropertyChanged(nameof(MessageBrush)); }
        }
        private void Error(string message)
        {
            MessageBrush = Brushes.Red;
            Message = message;
        }
        private void Success(string message)
        {
            MessageBrush = Brushes.LightGreen;
            Message = message;
        }

        private void ClearMessage() => Message = "";

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; ClearMessage(); OnPropertyChanged(nameof(Username)); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; ClearMessage(); OnPropertyChanged(nameof(Password)); }
        }

        public RelayCommand Login { get; private set; }
        public RelayCommand Register { get; private set; }

        public event EventHandler OnRequestClose;

        private class UserData
        {
            public List<Word> Words = new List<Word>();
            public List<string> Synonyms = new List<string>();
            public List<QuizAttempt> DefinitionQuizAttempts = new List<QuizAttempt>();
            public List<QuizAttempt> SynonymQuizAttempts = new List<QuizAttempt>();
        }

        private class User
        {
            public string Username, PasswordHash, Data;
            public User(string username, string passwordHash, string data)
            { Username = username; PasswordHash = passwordHash; Data = data; }
        }

        private static readonly byte[] IV = new byte[]
        { 13, 37, 21, 37, 69, 0xd, 0xe, 0xa, 0xd, 0xb, 0xe, 0xe, 0xf, 0x1, 0x2, 0x3 };

        public LoginViewModel()
        {
            const string FileName = "storage.json";
            Register = new RelayCommand(e =>
            {
                ClearMessage();
                if (!File.Exists(FileName))
                {
                    var dataJson = JsonConvert.SerializeObject(new UserData());
                    var dataString = Base64Encode(
                        EncryptStringToBytes(dataJson, PasswordTo32Bytes(Password), IV));
                    var users = new List<User>() { new User(Username, Hash(Password), dataString) };
                    var usersJson = JsonConvert.SerializeObject(users);
                    File.WriteAllText(FileName, usersJson, Encoding.UTF8);
                }
                else
                {
                    var usersJson = File.ReadAllText(FileName, Encoding.UTF8);
                    var users = JsonConvert.DeserializeObject<List<User>>(usersJson);
                    for (int i = 0; i < users.Count; ++i)
                        if (users[i].Username == Username)
                        { Error("This username is already taken."); return; }
                    var dataJson = JsonConvert.SerializeObject(new UserData());
                    var dataString = Base64Encode(
                        EncryptStringToBytes(dataJson, PasswordTo32Bytes(Password), IV));
                    users.Add(new User(Username, Hash(Password), dataString));
                    usersJson = JsonConvert.SerializeObject(users);
                    File.WriteAllText(FileName, usersJson, Encoding.UTF8);
                }
                Success("Please log in to your new account.");
            });
            Login = new RelayCommand(e =>
            {
                ClearMessage();
                if (!File.Exists(FileName))
                { Message = "Database file does not exist. Register to create it."; return; }
                var usersJson = File.ReadAllText(FileName, Encoding.UTF8);
                var users = JsonConvert.DeserializeObject<List<User>>(usersJson);
                const string error = "Wrong username or password.";
                for (int i = 0; i < users.Count; ++i)
                {
                    var user = users[i];
                    if (user.Username == Username)
                    {
                        var hash = Hash(Password);
                        if (hash != user.PasswordHash)
                        { Error(error); return; }
                        var data = JsonConvert.DeserializeObject<UserData>(
                            DecryptStringFromBytes(Base64Decode(user.Data), PasswordTo32Bytes(Password), IV));
                        OnRequestClose(this, new EventArgs());
                        return;
                    }
                }
                Error(error);
            });
        }

        private string Hash(string password)
        {
            if (password == null) password = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                // hash obliczony sha256 zawsze ma dokładnie 32 bajty (32 * 8 b = 256 b)
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // private string BytesToString(byte[] bytes) => Encoding.UTF8.GetString(bytes);
        private byte[] PasswordTo32Bytes(string password)
        {
            if (password == null) password = "";
            var bytes = Encoding.UTF8.GetBytes(password);
            if (bytes.Length == 32) return bytes;
            int i;
            if (bytes.Length < 32)
            {
                var longerBytes = new byte[32];
                for (i = 0; i < bytes.Length; ++i) longerBytes[i] = bytes[i];
                for (; i < 32; ++i) longerBytes[i] = 0x00;
                return longerBytes;
            }
            // if (bytes.Length > 32)
            var shorterBytes = new byte[32];
            for (i = 0; i < 32; ++i) shorterBytes[i] = bytes[i];
            return shorterBytes;
        }
        private string Base64Encode(byte[] bytes) => Convert.ToBase64String(bytes);
        private byte[] Base64Decode(string base64) => Convert.FromBase64String(base64);

        private byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        // Write all data to the stream.
                        swEncrypt.Write(plainText);
                    encrypted = msEncrypt.ToArray();
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");
            // Declare the string used to hold the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    // Read the decrypted bytes from the decrypting stream  and place them in a string.
                    plaintext = srDecrypt.ReadToEnd();
            }
            return plaintext;
        }
    }
}
