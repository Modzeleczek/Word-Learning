using System.Windows;
using System.Windows.Controls;

namespace Word_Learning.MVVM.View.Controls
{
    public partial class StatisticField : UserControl
    {
        public StatisticField()
        {
            InitializeComponent();
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(StatisticField), new PropertyMetadata(string.Empty));

        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }

        public static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register("ValueText", typeof(string), typeof(StatisticField), new PropertyMetadata(string.Empty));
    }
}
