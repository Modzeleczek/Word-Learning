using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class StatisticsViewModel : ObservableObject
    {
        private Window window;
        private int downloaded;
        public int Downloaded
        {
            get { return downloaded; }
            set { downloaded = value; OnPropertyChanged(nameof(Downloaded)); }
        }
        private int learned;
        public int Learned
        {
            get { return learned; }
            set { learned = value; OnPropertyChanged(nameof(Learned)); }
        }
        private int definitionMatches;
        public int DefinitionMatches
        {
            get { return definitionMatches; }
            set { definitionMatches = value; OnPropertyChanged(nameof(DefinitionMatches)); }
        }
        private int synonymMatches;
        public int SynonymMatches
        {
            get { return synonymMatches; }
            set { synonymMatches = value; OnPropertyChanged(nameof(SynonymMatches)); }
        }
        private int definitionAttempts;
        public int DefinitionAttempts
        {
            get { return definitionAttempts; }
            set { definitionAttempts = value; OnPropertyChanged(nameof(DefinitionAttempts)); }
        }
        private int synonymAttempts;
        public int SynonymAttempts
        {
            get { return synonymAttempts; }
            set { synonymAttempts = value; OnPropertyChanged(nameof(SynonymAttempts)); }
        }

        public RelayCommand ResetProgress { get; private set; }

        public StatisticsViewModel(Window window)
        {
            this.window = window;
            ResetProgress = new RelayCommand(e =>
            {
                var vm = new OkCancelViewModel { MessageText = "Do you want to reset your progress? " };
                new OkCancelWindow(window, vm).ShowDialog();
                var status = vm.Status;
                if (status.Code == 0)
                {
                    User.Instance.Clear();
                    Refresh();
                }
            });
        }

        public void Refresh()
        {
            var user = User.Instance;
            Downloaded = user.Words.Count;
            int learned = 0;
            int definitionMatches = 0;
            int synonymMatches = 0;
            foreach (var w in user.Words)
            {
                if (w.DefinitionMatches >= 4) ++learned;
                definitionMatches += w.DefinitionMatches;
                synonymMatches += w.SynonymMatches;
            }
            Learned = learned;
            DefinitionMatches = definitionMatches;
            SynonymMatches = synonymMatches;
            DefinitionAttempts = user.DefinitionQuizAttempts.Count;
            SynonymAttempts = user.SynonymQuizAttempts.Count;
        }
    }
}
