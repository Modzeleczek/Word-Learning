namespace Word_Learning.MVVM.Model
{
    public class DetailedAttempt
    {
        public QuizAttempt QuizAttempt { get; set; }
        public Word Question { get; set; }
        public Word[] Answers { get; set; } = new Word[4];
        public Word CorrectAnswer { get; set; }
        public Word UsersAnswer { get; set; }
    }
}
