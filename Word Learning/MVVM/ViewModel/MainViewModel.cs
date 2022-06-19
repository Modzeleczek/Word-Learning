using System.ComponentModel;
using Word_Learning.Core;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand SwitchToLearning { get; private set; }
        public RelayCommand SwitchToStatistics { get; private set; }
        public RelayCommand SwitchToDefinitionQuiz { get; private set; }
        private object currentView;
        public object CurrentModeVM
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentModeVM)); }
        }

        public MainViewModel()
        {
            var learningVM = new LearningViewModel();
            var statisticsVM = new StatisticsViewModel();
            var definitionQuizVM = new DefinitionQuizViewModel();
            CurrentModeVM = learningVM;
            SwitchToLearning = new RelayCommand(o => { if (CurrentModeVM != learningVM) CurrentModeVM = learningVM; });
            SwitchToStatistics = new RelayCommand(o => { if (CurrentModeVM != statisticsVM) CurrentModeVM = statisticsVM; });
            SwitchToDefinitionQuiz = new RelayCommand(o =>
            {
                if (CurrentModeVM == definitionQuizVM) return;
                definitionQuizVM.GenerateQuestion();
                CurrentModeVM = definitionQuizVM;
            });
            ShowLoginDialog();
        }

        private void ShowLoginDialog()
        {
            var loginViewModel = new LoginViewModel();
            var loginWindow = new LoginWindow { DataContext = loginViewModel };
            CancelEventHandler preventClose = (s, e) => { e.Cancel = true; };
            loginWindow.Closing += preventClose;
            loginViewModel.OnRequestClose += (s, e) =>
            {
                loginWindow.Closing -= preventClose;
                loginWindow.Close();
            };
            loginWindow.ShowDialog();
        }
    }
}
