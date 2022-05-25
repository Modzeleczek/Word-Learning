using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Word_Learning.MVVM.Model
{
    public class WordStorage
    {
        private static readonly WordStorage instance = new WordStorage();
        public static WordStorage Instance { get { return instance; } }
        public List<Word> DownloadedWords { get; } = new List<Word>();
        public List<Word> UsersWords { get; } = new List<Word>();
        private const string FileName = "word_storage.json";

        private WordStorage()
        {
            DownloadedWords.Add(new Word("car", "noun", "A vehicle.", "I can drive a car.", new string[] { }, 0));
            DownloadedWords.Add(new Word("computer", "noun", "A complex calculator.", "My friend has a computer.", new string[] { }, 1));
            DownloadedWords.Add(new Word("a", "aa", "aaa", "aaaa", new string[] { "aaaaa", "aaaaaa" }, 2));
            DownloadedWords.Add(new Word("b", "bb", "bbb", "bbbb", new string[] { "bbbbb" }, 3));
            SaveToFile();
        }

        private void SaveToFile()
        {
            var jsonString = JsonConvert.SerializeObject(DownloadedWords, Formatting.Indented);
            File.WriteAllText(FileName, jsonString, Encoding.UTF8);
        }

        private void LoadFromFile()
        {
            if (!File.Exists(FileName)) return;
            var jsonString = File.ReadAllText(FileName, Encoding.UTF8);
            var readWords = JsonConvert.DeserializeObject<List<Word>>(jsonString);
            DownloadedWords.Clear();
            foreach (var w in readWords)
                DownloadedWords.Add(w);
        }
    }
}
