using System.Collections.Generic;

namespace Word_Learning.MVVM.Model
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public static User Instance { get; set; } = new User();
        public List<Word> Words { get; set; } = new List<Word>();
        public List<string> Synonyms { get; set; } = new List<string>();
        public List<QuizAttempt> DefinitionQuizAttempts { get; set; } = new List<QuizAttempt>();
        public List<QuizAttempt> SynonymQuizAttempts { get; set; } = new List<QuizAttempt>();
        // public List<Word> UserWords { get; } = new List<Word>();

        public User() { }

        public void SaveToFile() => Storage.UpdateUser(this);

        public void Clear()
        {
            Username = "";
            Password = "";
            Words.Clear();
            Synonyms.Clear();
            DefinitionQuizAttempts.Clear();
            SynonymQuizAttempts.Clear();
        }
    }
}
