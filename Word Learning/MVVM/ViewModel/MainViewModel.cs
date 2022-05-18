using Word_Learning.Core;

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
        }
    }
}
