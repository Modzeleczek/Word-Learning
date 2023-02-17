using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;
using Word_Learning.MVVM.View;

namespace Word_Learning.MVVM.ViewModel
{
    public class LearningViewModel : ObservableObject
    {
        private ObservableCollection<Word> words;
        public ObservableCollection<Word> Words
        {
            get { return words; }
            private set { words = value; OnPropertyChanged(nameof(Words)); }
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
                    CurrentWordVM.Word = Words[selectedIndex];
                else
                    CurrentWordVM.Word = null;
                OnPropertyChanged(nameof(CurrentWordVM));
            }
        }
        public WordDetailsViewModel CurrentWordVM { get; } = new WordDetailsViewModel();

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
                var user = User.Instance;
                Words.Clear();
                do
                {
                    var s = value;
                    if (s.StartsWith("know:") && s.Length >= 6 && s[5] >= '0' && s[5] <= '4')
                    {
                        var level = value[5] - '0';
                        foreach (var w in user.Words)
                            if (w.DefinitionMatches == level) words.Add(w);
                        break;
                    }
                    foreach (var w in User.Instance.Words)
                        if (w.Content.Contains(searchText))
                            Words.Add(w);
                }
                while (false);
                SelectedIndex = -1;
            }
        }

        public RelayCommand DownloadWords { get; }

        public LearningViewModel(Window window)
        {
            Words = new ObservableCollection<Word>(User.Instance.Words);
            SearchText = "";
            DownloadWords = new RelayCommand(_e =>
            {
                var downloadViewModel = new DownloadViewModel();
                var downloadWindow = new DownloadWindow { DataContext = downloadViewModel, Owner = window };
                CancelEventHandler handler = (s, e) => e.Cancel = true;
                downloadWindow.Closing += handler;
                downloadViewModel.OnRequestClose += (s, e) =>
                {
                    downloadWindow.Closing -= handler;
                    downloadWindow.Close();
                };
                downloadWindow.ShowDialog();
                var status = downloadViewModel.Status;
                if (status.Code != 0) MessageWindow.BadDialog(window, status.Message);
                else if (status.Message != "") MessageWindow.GoodDialog(window, status.Message);
                Words = new ObservableCollection<Word>(User.Instance.Words);
            });
        }

        public void Refresh()
        {
            Words = new ObservableCollection<Word>(User.Instance.Words);
        }
    }
}
