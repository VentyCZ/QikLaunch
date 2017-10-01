using System.Windows;

namespace QikLaunch
{
    /// <summary>
    /// Interaction logic for QuickLaunch.xaml
    /// </summary>
    public partial class Spotlight : Window
    {
        public Spotlight()
        {
            InitializeComponent();
            this.Beautify();
            
            Loaded += QuickLaunch_Loaded;
        }

        /// <summary>
        /// When the window is loaded
        /// </summary>
        /// <param name="sender">This window</param>
        /// <param name="e">EventArgs</param>
        private void QuickLaunch_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Move the window to the right place
        }
    }
}
