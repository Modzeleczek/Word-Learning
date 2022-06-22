using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class AttemptDetailsViewModel : ObservableObject
    {
        private DetailedAttempt detailedAttempt = null;
        public DetailedAttempt DetailedAttempt
        {
            get { return detailedAttempt; }
            set { detailedAttempt = value; OnPropertyChanged(nameof(DetailedAttempt)); }
        }

        public AttemptDetailsViewModel() { }
    }
}
