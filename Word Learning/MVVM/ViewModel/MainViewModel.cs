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
        private RelayCommand switchToLearning;
        public RelayCommand SwitchToLearning
        {
            get { return switchToLearning; }
            private set { switchToLearning = value; OnPropertyChanged(nameof(SwitchToLearning)); }
        }
        private RelayCommand switchToDefinitionQuiz;
        public RelayCommand SwitchToDefinitionQuiz
        {
            get { return switchToDefinitionQuiz; }
            private set { switchToDefinitionQuiz = value; OnPropertyChanged(nameof(SwitchToDefinitionQuiz)); }
        }
        private RelayCommand switchToSynonymQuiz;
        public RelayCommand SwitchToSynonymQuiz
        {
            get { return switchToSynonymQuiz; }
            private set { switchToSynonymQuiz = value; OnPropertyChanged(nameof(SwitchToSynonymQuiz)); }
        }
        private RelayCommand switchToStatistics;
        public RelayCommand SwitchToStatistics
        {
            get { return switchToStatistics; }
            private set { switchToStatistics = value; OnPropertyChanged(nameof(SwitchToStatistics)); }
        }
        private object currentView;
        public object CurrentModeVM
        {
            get { return currentView; }
            private set { currentView = value; OnPropertyChanged(nameof(CurrentModeVM)); }
        }
        private RelayCommand logout;
        public RelayCommand Logout
        {
            get { return logout; }
            private set { logout = value; OnPropertyChanged(nameof(Logout)); }
        }
        private RelayCommand close;
        public RelayCommand Close
        {
            get { return close; }
            private set { close = value; OnPropertyChanged(nameof(Close)); }
        }

        private LearningViewModel learningVM;
        private DefinitionQuizViewModel definitionQuizVM;
        private SynonymQuizViewModel synonymQuizVM;
        private StatisticsViewModel statisticsVM;

        public MainViewModel()
        {
            WindowLoaded = new RelayCommand(windowLoadedE =>
            {
                var window = (Window)windowLoadedE;
                CreateViewModels((Window)windowLoadedE);
                SwitchToLearning = new RelayCommand(e =>
                {
                    if (CurrentModeVM == learningVM) return;
                    CurrentModeVM = learningVM;
                    if (User.Instance.Words.Count == 0)
                        MessageWindow.NeutralDialog(window, "You don't have any words. " +
                            "Download some by clicking the button near search box.");
                });
                SwitchToDefinitionQuiz = new RelayCommand(e =>
                {
                    if (CurrentModeVM == definitionQuizVM) return;
                    var status = definitionQuizVM.GenerateQuiz();
                    if (status.Code != 0)
                        new MessageWindow(window, MessageViewModel.Bad(status.Message)).ShowDialog();
                    else
                        CurrentModeVM = definitionQuizVM;
                });
                switchToSynonymQuiz = new RelayCommand(e =>
                {
                    if (CurrentModeVM == synonymQuizVM) return;
                    // synonymQuizVM.GenerateQuestion();
                    CurrentModeVM = synonymQuizVM;
                });
                SwitchToStatistics = new RelayCommand(e =>
                {
                    if (CurrentModeVM != statisticsVM) CurrentModeVM = statisticsVM;
                });
                Logout = new RelayCommand(e =>
                {
                    User.Instance.SaveToFile();
                    User.Instance.Clear();
                    CurrentModeVM = null;
                    ShowLoginDialog(window);
                });
                Close = new RelayCommand(e =>
                {
                    User.Instance.SaveToFile();
                });
                Logout.Execute();
                SwitchToLearning.Execute();
            });
        }

        private void CreateViewModels(Window window)
        {
            learningVM = new LearningViewModel(window);
            definitionQuizVM = new DefinitionQuizViewModel(window);
            synonymQuizVM = new SynonymQuizViewModel(window);
            statisticsVM = new StatisticsViewModel(window);
        }

        private void ShowLoginDialog(Window owner)
        {
            var loginViewModel = new LoginViewModel();
            var loginWindow = new LoginWindow { DataContext = loginViewModel, Owner = owner };
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
            CreateViewModels(owner);
            SwitchToLearning.Execute();
        }
    }
}
