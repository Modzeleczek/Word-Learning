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
        public ObservableCollection<Word> words;
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
                Words.Clear();
                foreach (var w in User.Instance.Words)
                    if (w.Content.Contains(searchText))
                        Words.Add(w);
                SelectedIndex = -1;
            }
        }

        private string wordCategory;
        public string WordCategory
        {
            get { return wordCategory; }
            set
            {
                wordCategory = value;
                OnPropertyChanged(nameof(WordCategory));
            }
        }
        public RelayCommand SwitchCategory { get; }

        public RelayCommand DownloadWords { get; }

        public LearningViewModel(Window window)
        {
            Words = new ObservableCollection<Word>(User.Instance.Words);
            SearchText = "";
            var wordCategories = new string[] { "Downloaded", "User's" };
            int wordCategoryId = 0;
            WordCategory = wordCategories[wordCategoryId];
            SwitchCategory = new RelayCommand(e =>
            {
                wordCategoryId = (wordCategoryId + 1) % 2;
                WordCategory = wordCategories[wordCategoryId];
            });
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
    }
}
