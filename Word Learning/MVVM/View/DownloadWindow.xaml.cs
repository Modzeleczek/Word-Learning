using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Word_Learning.MVVM.View
{
    public partial class DownloadWindow : Window
    {
        public DownloadWindow()
        {
            InitializeComponent();
            // Instantiate and initialize variable for normal Path Data without transformation:        
            Geometry geo = Geometry.Parse("M 0,0 l 150,0 0,10 -150,0 z");

            // Instantiate and initialize variable for desired shearing/transvection
            // (use opposite transformation to the one in the ControlTemplate):
            SkewTransform skewT = new SkewTransform(45, 0);
            // In case of additional transformations:
            // Instantiate and initialize variable for desired translation:
            //TranslateTransform transT = new TranslateTransform(-31.89, 0);
            // Instantiate variable for all transformations, as you have to apply all transformation at once:
            //TransformGroup tG = new TransformGroup();
            //tG.Children.Add(skewT);
            //tG.Children.Add(transT);

            // Create a clone of of your Geometry object,
            // since in order to apply a transform, geometry must not be readonly:
            Geometry geoClone = geo.Clone();

            // Apply transformation:
            geoClone.Transform = skewT;
            // For multiple transformations:
            //geoClone.Transform = tG;

            // Calculate new Path Data:
            string result = geoClone.GetFlattenedPathGeometry(0.001, ToleranceType.Relative).ToString(CultureInfo.InvariantCulture);
            //var result = geoClone.GetFlattenedPathGeometry(0.001, ToleranceType.Relative).ToString().Replace(",", ".").Replace(";", ",");

            // Return new Path Data:
            Trace.WriteLine(this + ": " + result);
            // Returns: M0,0L150,0 160,10 10,10 0,0z
            // Note that returned values are absolute values.
            // Identical Path Data in relative coordinates (meaning offset values to respective previous point):
            // M 0,0 l 150,0 10,10 -150,0 z

        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
