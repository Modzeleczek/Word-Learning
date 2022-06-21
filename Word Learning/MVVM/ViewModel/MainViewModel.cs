using System.ComponentModel;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand Logout { get; }
        public RelayCommand SwitchToLearning { get; }
        public RelayCommand SwitchToStatistics { get; }
        public RelayCommand SwitchToDefinitionQuiz { get; }
        private object currentView;
        public object CurrentModeVM
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentModeVM)); }
        }

        public MainViewModel()
        {
            var learningVM = new LearningViewModel();
            var learningView = new LearningView { DataContext = learningVM };
            var statisticsVM = new StatisticsViewModel();
            var definitionQuizVM = new DefinitionQuizViewModel();
            SwitchToLearning = new RelayCommand(o =>
            {
                if (CurrentModeVM == learningVM) return;
                if (User.Instance.Words.Count <= 4)
                    new MessageWindow(new MessageViewModel
                    {
                        MessageText = "You don't have any words. Download some by clicking the button near search box.",
                        MessageFontSize = 16
                    }).ShowDialog();
                CurrentModeVM = learningVM;
            });
            SwitchToStatistics = new RelayCommand(o => { if (CurrentModeVM != statisticsVM) CurrentModeVM = statisticsVM; });
            SwitchToDefinitionQuiz = new RelayCommand(o =>
            {
                if (CurrentModeVM == definitionQuizVM) return;
                definitionQuizVM.GenerateQuestion();
                CurrentModeVM = definitionQuizVM;
            });
            Logout = new RelayCommand(e =>
            {
                User.Instance.Clear();
                ShowLoginDialog();
            });
            ShowLoginDialog();
            SwitchToLearning.Execute();
        }

        private void ShowLoginDialog()
        {
            var loginViewModel = new LoginViewModel();
            var loginWindow = new LoginWindow { DataContext = loginViewModel };
            CancelEventHandler handler = (s, e) =>
            {
                User.Instance.SaveToFile();
                Application.Current.Shutdown();
            };
            loginWindow.Closing += handler;
            loginViewModel.OnRequestClose += (s, e) =>
            {
                loginWindow.Closing -= handler;
                loginWindow.Close();
            };
            loginWindow.ShowDialog();
        }
    }
}
