using System.Collections.Generic;

namespace Word_Learning.MVVM.Model
{
    public class WordStorage
    {
        private static readonly WordStorage instance = new WordStorage();
        public static WordStorage Instance { get { return instance; } }
        public List<Word> Words { get; } = new List<Word>();

        private WordStorage()
        {
            Words.Add(new Word("car", "noun", "A vehicle.", "I can drive a car.", new string[] { }, 0));
            Words.Add(new Word("computer", "noun", "A complex calculator.", "My friend has a computer.", new string[] { }, 1));
            Words.Add(new Word("a", "aa", "aaa", "aaaa", new string[] { "aaaaa", "aaaaaa" }, 2));
            Words.Add(new Word("b", "bb", "bbb", "bbbb", new string[] { "bbbbb" }, 3));
        }
    }
}
