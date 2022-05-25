using System.Collections.ObjectModel;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class LearningViewModel : ObservableObject
    {
        public ObservableCollection<Word> Words { get; }
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
                foreach (var w in WordStorage.Instance.DownloadedWords)
                    if (w.Word_.Contains(searchText))
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
        public RelayCommand SwitchCategoryCommand { get; }

        public LearningViewModel()
        {
            Words = new ObservableCollection<Word>(WordStorage.Instance.DownloadedWords);
            SearchText = "";
            var wordCategories = new string[] { "Downloaded", "User's" };
            int wordCategoryId = 0;
            WordCategory = wordCategories[wordCategoryId];
            SwitchCategoryCommand = new RelayCommand(e =>
            {
                wordCategoryId = (wordCategoryId + 1) % 2;
                WordCategory = wordCategories[wordCategoryId];
            });
        }
    }
}
