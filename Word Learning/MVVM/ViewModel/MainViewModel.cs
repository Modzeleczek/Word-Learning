using System.ComponentModel;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand WindowLoaded { get; }
        public RelayCommand Logout { get; private set; }
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
            WindowLoaded = new RelayCommand(windowLoadedE =>
            {
                var window = (Window)windowLoadedE;
                var learningVM = new LearningViewModel(window);
                var learningView = new LearningView { DataContext = learningVM };
                var statisticsVM = new StatisticsViewModel(window);
                var definitionQuizVM = new DefinitionQuizViewModel(window);
                SwitchToLearning = new RelayCommand(e =>
                {
                    if (CurrentModeVM == learningVM) return;
                    if (User.Instance.Words.Count == 0)
                        new MessageWindow(window, new MessageViewModel
                        {
                            MessageText = "You don't have any words. Download some by clicking the button near search box.",
                            MessageFontSize = 16
                        }).ShowDialog();
                    CurrentModeVM = learningVM;
                });
                SwitchToStatistics = new RelayCommand(e =>
                { if (CurrentModeVM != statisticsVM) CurrentModeVM = statisticsVM; });
                SwitchToDefinitionQuiz = new RelayCommand(e =>
                {
                    if (CurrentModeVM == definitionQuizVM) return;
                    definitionQuizVM.GenerateQuestion();
                    CurrentModeVM = definitionQuizVM;
                });
                Logout = new RelayCommand(e =>
                {
                    User.Instance.Clear();
                    ShowLoginDialog(window);
                });
                ShowLoginDialog(window);
                SwitchToLearning.Execute();
            });
        }

        private void ShowLoginDialog(Window mainWindow)
        {
            var loginViewModel = new LoginViewModel();
            var loginWindow = new LoginWindow { DataContext = loginViewModel, Owner = mainWindow };
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
