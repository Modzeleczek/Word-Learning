namespace Word_Learning.MVVM.Model
{
    public class QuizAttempt
    {
        public int QuestionId { get; set; }
        public int[] AnswerIds { get; set; } = new int[4];
        public int CorrectAnswerId { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}
