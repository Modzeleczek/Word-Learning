using System.Windows.Media;
using Word_Learning.Core;

namespace Word_Learning.MVVM.ViewModel
{
    class MessageViewModel : ObservableObject
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

        private string buttonText = "OK";
        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; OnPropertyChanged(nameof(ButtonText)); }
        }

        private int buttonFontSize = 16;
        public int ButtonFontSize
        {
            get { return buttonFontSize; }
            set { buttonFontSize = value; OnPropertyChanged(nameof(ButtonFontSize)); }
        }

        private Color buttonGradientStartColor = Color.FromArgb(255, 0xA5, 0xFF, 0x1F);
        public Color ButtonGradientStartColor
        {
            get { return buttonGradientStartColor; }
            set { buttonGradientStartColor = value; OnPropertyChanged(nameof(ButtonGradientStartColor)); }
        }

        private Color buttonGradientEndColor = Color.FromArgb(255, 0x74, 0xC2, 0x00);
        public Color ButtonGradientEndColor
        {
            get { return buttonGradientEndColor; }
            set { buttonGradientEndColor = value; OnPropertyChanged(nameof(ButtonGradientEndColor)); }
        }
    }
}
