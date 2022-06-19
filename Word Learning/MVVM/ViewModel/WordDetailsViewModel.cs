using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class WordDetailsViewModel : ObservableObject
    {
        private Word word = null;
        public Word Word
        {
            get { return word; }
            set { word = value; OnPropertyChanged(nameof(Word)); }
        }

        public WordDetailsViewModel() { }
    }
}
