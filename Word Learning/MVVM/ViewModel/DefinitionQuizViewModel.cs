using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class DefinitionQuizViewModel : ObservableObject
    {
        private Window window;

        private Word word = null;
        public Word Word
        {
            get { return word; }
            set { word = value; OnPropertyChanged(nameof(Word)); }
        }
        public string[] Answer { get; } = new string[4];
        public RelayCommand[] AnswerClick { get; } = new RelayCommand[4];

        public DefinitionQuizViewModel(Window window) { this.window = window; }

        public void GenerateQuestion()
        {
            var wordsDB = User.Instance.Words;
            Word = wordsDB[0];
            Answer[0] = wordsDB[2].Content;
            Answer[1] = wordsDB[1].Content;
            Answer[2] = wordsDB[3].Content;
            Answer[3] = wordsDB[0].Content;
            int correctAnswer = 3;
            for (int i = 0; i < 4; ++i)
            {
                int iCopy = i;
                AnswerClick[i] = new RelayCommand(o =>
                {
                    if (iCopy == correctAnswer)
                        new MessageWindow(window, MessageViewModel.Good("Correct answer!")).ShowDialog();
                    else
                        new MessageWindow(window, MessageViewModel.Bad("Incorrect answer.")).ShowDialog();
                });
            }
        }
    }
}
