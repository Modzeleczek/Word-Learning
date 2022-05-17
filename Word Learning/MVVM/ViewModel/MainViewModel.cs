using Word_Learning.Core;

namespace Word_Learning.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand SwitchToLearning { get; private set; }

        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(currentView)); }
        }

        public MainViewModel()
        {
            var learningVM = new LearningViewModel();
            SwitchToLearning = new RelayCommand(o =>
            {
                CurrentView = learningVM;
            });
        }
    }
}
