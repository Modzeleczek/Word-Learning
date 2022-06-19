using System.Windows;
using System.Windows.Media;
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
            set { word = value; OnPropertyChanged(nameof(Word)); }
        }
        public string[] Answer { get; } = new string[4];
        public RelayCommand[] AnswerClick { get; } = new RelayCommand[4];

        public DefinitionQuizViewModel() { }

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
                    {
                        var vm = new MessageViewModel
                        {
                            MessageText = "Correct answer!",
                            ButtonGradientStartColor = Color.FromArgb(255, 0xA5, 0xFF, 0x1F),
                            ButtonGradientEndColor = Color.FromArgb(255, 0x74, 0xC2, 0x00)
                        };
                        var window = new MessageWindow
                        {
                            DataContext = vm,
                            Owner = Application.Current.MainWindow,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        window.ShowDialog();
                    }
                    else
                    {
                        var vm = new MessageViewModel
                        {
                            MessageText = "Incorrect answer.",
                            ButtonGradientStartColor = Color.FromArgb(255, 0xFF, 0x70, 0x70),
                            ButtonGradientEndColor = Color.FromArgb(255, 0xFF, 0x52, 0x52)
                        };
                        var window = new MessageWindow
                        {
                            DataContext = vm,
                            Owner = Application.Current.MainWindow,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        window.ShowDialog();
                    }
                });
            }
        }
    }
}
