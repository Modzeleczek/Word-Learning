using System.Collections.ObjectModel;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class DefinitionQuizHistoryViewModel : ObservableObject
    {
        // Static attempts read from database
        private DetailedAttempt[] StaticDetailedAttempts;
        private ObservableCollection<DetailedAttempt> detailedAttempts;
        // Filtered attempts
        public ObservableCollection<DetailedAttempt> DetailedAttempts
        {
            get { return detailedAttempts; }
            private set { detailedAttempts = value; OnPropertyChanged(nameof(DetailedAttempts)); }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
                if (selectedIndex != -1)
                    CurrentAttemptVM.DetailedAttempt = DetailedAttempts[selectedIndex];
                else 
                    CurrentAttemptVM.DetailedAttempt = null;
                OnPropertyChanged(nameof(CurrentAttemptVM));
            }
        }
        public AttemptDetailsViewModel CurrentAttemptVM { get; } = new AttemptDetailsViewModel();

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
                DetailedAttempts.Clear();
                foreach (var da in StaticDetailedAttempts)
                    if (da.Question.Content.Contains(searchText))
                        DetailedAttempts.Add(da);
                SelectedIndex = -1;
            }
        }

        public DefinitionQuizHistoryViewModel()
        {
            Refresh();
            SearchText = "";
        }

        public void Refresh()
        {
            var user = User.Instance;
            StaticDetailedAttempts = new DetailedAttempt[user.DefinitionQuizAttempts.Count];
            int sdaI = 0;
            foreach (var qa in user.DefinitionQuizAttempts)
            {
                var da = new DetailedAttempt
                {
                    QuizAttempt = qa,
                    Question = user.Words[qa.QuestionId]
                };
                for (int i = 0; i < 4; ++i)
                    da.Answers[i] = user.Words[qa.AnswerIds[i]];
                da.CorrectAnswer = da.Answers[qa.CorrectAnswerIndex];
                da.UsersAnswer = da.Answers[qa.SelectedAnswerIndex];
                StaticDetailedAttempts[sdaI++] = da;
            }
            DetailedAttempts = new ObservableCollection<DetailedAttempt>(StaticDetailedAttempts);
        }
    }
}
