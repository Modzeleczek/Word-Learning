using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class DefinitionQuizViewModel : ObservableObject
    {
        public class IdWord
        {
            public int Id { get; set; }
            public Word Word { get; set; }
            public IdWord(int id, Word word) { Id = id; Word = word; }
        }

        private string stringQuestion = null;
        public string StringQuestion
        {
            get { return stringQuestion; }
            set { stringQuestion = value; OnPropertyChanged(nameof(StringQuestion)); }
        }
        public ObservableCollection<string> StringAnswers { get; }
        private IdWord question;
        private IdWord[] answers;
        private int correctAnswerIndex;
        public RelayCommand[] AnswerClick { get; }
        private Random rng;

        public DefinitionQuizViewModel(Window window)
        {
            StringAnswers = new ObservableCollection<string>();
            for (int i = 0; i < 4; ++i)
                StringAnswers.Add("");
            AnswerClick = new RelayCommand[4];
            rng = new Random();
            for (int i = 0; i < 4; ++i)
            {
                int answerIndex = i;
                AnswerClick[i] = new RelayCommand(o =>
                {
                    if (question != null) // quiz jest ustawiony
                    {
                        var user = User.Instance;
                        if (answers[answerIndex].Id == question.Id)
                        {
                            var word = user.Words[question.Id];
                            if (word.DefinitionMatches < 4)
                                ++word.DefinitionMatches;
                            new MessageWindow(window,
                                MessageViewModel.Good("Correct answer!")).ShowDialog();
                        }
                        else
                        {
                            user.Words[question.Id].DefinitionMatches = 0;
                            new MessageWindow(window,
                                MessageViewModel.Bad("Incorrect answer.")).ShowDialog();
                        }
                        var attempt = new QuizAttempt
                        {
                            QuestionId = question.Id,
                            CorrectAnswerIndex = correctAnswerIndex,
                            SelectedAnswerIndex = answerIndex,
                            Time = DateTime.Now
                        };
                        for (int aI = 0; aI < 4; ++aI)
                            attempt.AnswerIds[aI] = answers[aI].Id;
                        user.DefinitionQuizAttempts.Add(attempt);
                    }
                    GenerateQuiz();
                });
            }
        }

        private void Clear()
        {
            question = null;
            StringQuestion = "";
            for (int i = 0; i < 4; ++i)
                StringAnswers[i] = "";
        }

        public Status GenerateQuiz()
        {
            var all = User.Instance.Words;
            var unknownLL = new LinkedList<IdWord>();
            for (int i = 0; i < all.Count; ++i)
            {
                var w = all[i];
                if (w.DefinitionMatches < 4)
                    unknownLL.AddLast(new IdWord(i, w));
            }
            if (unknownLL.Count == 0)
            { Clear(); return new Status(1, "No more words to learn. Download new ones."); }
            var unknownL = new List<IdWord>(unknownLL);
            question = PickNRandom(unknownL, 1)[0];
            if (all.Count < 4)
            // w bazie danych nie ma nawet 4 słów
            { Clear(); return new Status(2, "Not enough words for quiz answers. Download new ones."); }
            var allIdWordsL = new List<IdWord>(all.Count);
            for (int i = 0; i < all.Count; ++i)
                allIdWordsL.Add(new IdWord(i, all[i]));
            answers = GenerateAnswers(question, allIdWordsL);
            StringQuestion = question.Word.Definition;
            for (int i = 0; i < 4; ++i)
                StringAnswers[i] = answers[i].Word.Content;
            return new Status(0, "");
        }

        private List<IdWord> PickNRandom(List<IdWord> list, int n)
        {
            int length = list.Count;
            if (length < n) return null;
            for (int i = length - 1; i >= length - n; --i)
            {
                var randomIndex = rng.Next(0, i + 1);
                var temp = list[randomIndex];
                list[randomIndex] = list[i];
                list[i] = temp;
            }
            var ret = new List<IdWord>(n);
            for (int i = length - n; i < length; ++i)
                ret.Add(list[i]);
            return ret;
        }

        private IdWord[] GenerateAnswers(IdWord question, List<IdWord> all)
        {
            all.RemoveAt(question.Id);
            List<IdWord> incorrectAnswers = PickNRandom(all, 3);
            var answers = new IdWord[4];
            correctAnswerIndex = rng.Next(4);
            int i;
            int aI = 0;
            for (i = 0; i < correctAnswerIndex; ++i)
                answers[i] = incorrectAnswers[aI++];
            answers[i++] = question; // prawidłowa odpowiedź
            for (; i < 4; ++i)
                answers[i] = incorrectAnswers[aI++];
            return answers;
        }
    }
}
