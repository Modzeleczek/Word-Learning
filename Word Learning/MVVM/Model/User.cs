using System.Collections.Generic;

namespace Word_Learning.MVVM.Model
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        private static readonly User instance = new User();
        public static User Instance { get { return instance; } }
        public List<Word> Words { get; } = new List<Word>();
        public List<string> Synonyms { get; } = new List<string>();
        public List<QuizAttempt> DefinitionQuizAttempts = new List<QuizAttempt>();
        public List<QuizAttempt> SynonymQuizAttempts = new List<QuizAttempt>();
        // public List<Word> UserWords { get; } = new List<Word>();

        private User()
        {
            Words.Add(new Word("car", "noun", "A vehicle.", "I can drive a car.", new int[] { }, 0, 0));
            Words.Add(new Word("computer", "noun", "A complex calculator.", "My friend has a computer.", new int[] { }, 1, 0));
            Words.Add(new Word("a", "aa", "aaa", "aaaa", new int[] { 0, 1 }, 2, 0));
            Words.Add(new Word("b", "bb", "bbb", "bbbb", new int[] { 2 }, 3, 0));
        }

        
    }
}
