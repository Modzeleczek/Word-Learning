using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word_Learning.Core;
using Word_Learning.MVVM.Model;

namespace Word_Learning.MVVM.ViewModel
{
    public class OkCancelViewModel : ObservableObject
    {
        private string messageText = "Message";
        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(nameof(MessageText)); }
        }

        private int messageFontSize = 16;
        public int MessageFontSize
        {
            get { return messageFontSize; }
            set { messageFontSize = value; OnPropertyChanged(nameof(MessageFontSize)); }
        }

        private int buttonFontSize = 16;
        public int ButtonFontSize
        {
            get { return buttonFontSize; }
            set { buttonFontSize = value; OnPropertyChanged(nameof(ButtonFontSize)); }
        }

        public Status Status { get; private set; }
        public RelayCommand Yes { get; private set; }
        public RelayCommand No { get; private set; }

        public OkCancelViewModel()
        {
            Yes = new RelayCommand(e =>
            {
                Status = new Status(0, "");
            });
            No = new RelayCommand(e =>
            {
                Status = new Status(1, "");
            });
        }
    }
}
