using System;

namespace Word_Learning.MVVM.Model
{
    public class QuizAttempt
    {
        public int QuestionId { get; set; }
        public int[] AnswerIds { get; set; } = new int[4];
        public int CorrectAnswerIndex { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public DateTime Time { get; set; }
    }
}
