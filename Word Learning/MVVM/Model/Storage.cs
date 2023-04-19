using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Word_Learning.MVVM.Model
{
    class Storage
    {
        private class StorageUserData
        {
            public List<Word> Words;
            public List<string> Synonyms;
            public List<QuizAttempt> DefinitionQuizAttempts;
            public List<QuizAttempt> SynonymQuizAttempts;

            public StorageUserData() { }

            public StorageUserData(User user)
            {
                Words = user.Words;
                Synonyms = user.Synonyms;
                DefinitionQuizAttempts = user.DefinitionQuizAttempts;
                SynonymQuizAttempts = user.SynonymQuizAttempts;
            }
        }

        private class StorageUser
        {
            public string Username, PasswordHash, Data;
            public StorageUser(string username, string passwordHash, string data)
            { Username = username; PasswordHash = passwordHash; Data = data; }
        }

        private static readonly string FileName = "storage.json";
        private static readonly byte[] IV = new byte[]
        { 13, 37, 21, 37, 69, 0xd, 0xe, 0xa, 0xd, 0xb, 0xe, 0xe, 0xf, 0x1, 0x2, 0x3 };

        public static Status AddUser(User user)
        {
            List<StorageUser> users;
            string usersJson;
            if (File.Exists(FileName))
            {
                usersJson = File.ReadAllText(FileName, Encoding.UTF8);
                users = JsonConvert.DeserializeObject<List<StorageUser>>(usersJson);
            }
            else users = new List<StorageUser>();
            for (int i = 0; i < users.Count; ++i)
                if (users[i].Username == user.Username) return new Status(1, "This username is already taken.");
            var dataJson = JsonConvert.SerializeObject(new StorageUserData(user));
            var dataString = Base64Encode(
                EncryptStringToBytes(dataJson, PasswordTo32Bytes(user.Password), IV));
            users.Add(new StorageUser(user.Username, Hash(user.Password), dataString));
            usersJson = JsonConvert.SerializeObject(users);
            File.WriteAllText(FileName, usersJson, Encoding.UTF8);
            return new Status(0);
        }

        public static Status GetUser(string username, string password, out User user)
        {
            user = null;
            if (!File.Exists(FileName))
                return new Status(1, "Database file does not exist. Register to create it.");
            var usersJson = File.ReadAllText(FileName, Encoding.UTF8);
            var users = JsonConvert.DeserializeObject<List<StorageUser>>(usersJson);
            const string error = "Wrong username or password.";
            var passwordHash = Hash(password);
            for (int i = 0; i < users.Count; ++i)
            {
                var readUser = users[i];
                if (readUser.Username == username)
                {
                    if (passwordHash != readUser.PasswordHash) return new Status(2, error);
                    var data = JsonConvert.DeserializeObject<StorageUserData>(
                        DecryptStringFromBytes(Base64Decode(readUser.Data),
                        PasswordTo32Bytes(password), IV));
                    user = new User
                    {
                        Username = username,
                        Password = password,
                        Words = data.Words,
                        Synonyms = data.Synonyms,
                        DefinitionQuizAttempts = data.DefinitionQuizAttempts,
                        SynonymQuizAttempts = data.SynonymQuizAttempts
                    };
                    return new Status(0);
                }
            }
            return new Status(3, error);
        }

        public static Status UpdateUser(User user)
        {
            List<StorageUser> users;
            string usersJson;
            if (File.Exists(FileName))
            {
                usersJson = File.ReadAllText(FileName, Encoding.UTF8);
                users = JsonConvert.DeserializeObject<List<StorageUser>>(usersJson);
            }
            else users = new List<StorageUser>();
            var passwordHash = Hash(user.Password);
            for (int i = 0; i < users.Count; ++i)
            {
                var readUser = users[i];
                if (readUser.Username == user.Username)
                {
                    if (passwordHash != readUser.PasswordHash)
                        return new Status(1, "Wrong username or password.");
                    var dataJson = JsonConvert.SerializeObject(new StorageUserData(user));
                    var dataString = Base64Encode(
                        EncryptStringToBytes(dataJson, PasswordTo32Bytes(user.Password), IV));
                    users[i].Data = dataString;
                    usersJson = JsonConvert.SerializeObject(users);
                    File.WriteAllText(FileName, usersJson, Encoding.UTF8);
                    return new Status(0);
                }
            }
            return new Status(2, "Wrong username or password.");
        }

        private static string Hash(string password)
        {
            if (password == null) password = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                /* Hash computed with SHA256 always has exactly 32 bytes
                (32 B = 32 * 8 b = 256 b). */
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // private string BytesToString(byte[] bytes) => Encoding.UTF8.GetString(bytes);
        private static byte[] PasswordTo32Bytes(string password)
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

        private static string Base64Encode(byte[] bytes) => Convert.ToBase64String(bytes);

        private static byte[] Base64Decode(string base64) => Convert.FromBase64String(base64);

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
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

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
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
