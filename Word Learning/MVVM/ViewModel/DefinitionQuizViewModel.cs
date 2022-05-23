using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class DefinitionQuizViewModel : ObservableObject
    {
        private Word word = null;
        public Word Word
        {
            get { return word; }
            set
            {
                word = value;
                OnPropertyChanged(nameof(Word));
            }
        }
        public string[] Answer { get; } = new string[4];
        public RelayCommand[] AnswerClick { get; } = new RelayCommand[4];

        public DefinitionQuizViewModel() { }

        public void GenerateQuestion()
        {
            var wordsDB = WordStorage.Instance.Words;
            Word = wordsDB[0];
            Answer[0] = wordsDB[2].Word_;
            Answer[1] = wordsDB[1].Word_;
            Answer[2] = wordsDB[3].Word_;
            Answer[3] = wordsDB[0].Word_;
            int correctAnswer = 3;
            for (int i = 0; i < 4; ++i)
            {
                int iCopy = i;
                AnswerClick[i] = new RelayCommand(o =>
                {
                    if (iCopy == correctAnswer)
                    {
                        CorrectCustomMessageBox AnswerWindow = new CorrectCustomMessageBox();
                        AnswerWindow.Owner = Application.Current.MainWindow;
                        AnswerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        AnswerWindow.ShowDialog();

                    }
                    else
                    {
                        IncorrectCustomMessageBox AnswerWindow = new IncorrectCustomMessageBox();
                        AnswerWindow.Owner = Application.Current.MainWindow;
                        AnswerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        AnswerWindow.ShowDialog();
                    }
                });
            }
        }
    }
}
