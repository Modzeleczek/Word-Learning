using Word_Learning.Core;

namespace Word_Learning.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand SwitchToLearning { get; private set; }

        private object currentView;
        public object CurrentModeVM
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentModeVM)); }
        }

        public MainViewModel()
        {
            var learningVM = new LearningViewModel();
            CurrentModeVM = learningVM;
            SwitchToLearning = new RelayCommand(o => CurrentModeVM = learningVM);
        }
    }
}
