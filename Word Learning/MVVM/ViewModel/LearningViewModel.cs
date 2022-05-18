using System.Collections.ObjectModel;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class LearningViewModel : ObservableObject
    {
        public ObservableCollection<Word> Words { get; } = new ObservableCollection<Word>();
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

        public LearningViewModel()
        {
            Words.Add(new Word("car", "noun", "A vehicle.", "I can drive a car.", new string[] { }, 0));
            Words.Add(new Word("computer", "noun", "A complex calculator.", "My friend has a computer.", new string[] { }, 1));
            Words.Add(new Word("a", "b", "c", "d", new string[] { "e", "f" }, 2));
            SelectedIndex = -1;
        }
    }
}
