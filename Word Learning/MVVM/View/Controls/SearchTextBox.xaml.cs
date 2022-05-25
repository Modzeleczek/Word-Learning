using System.Windows;
using System.Windows.Controls;

namespace Word_Learning.MVVM.View.Controls
{
    public partial class SearchTextBox : UserControl
    {
        public SearchTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SearchTextBox), new PropertyMetadata(string.Empty));

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var expression = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            expression.UpdateSource();
        }
    }
}
