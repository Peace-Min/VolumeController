using System.Windows;

namespace VolumeController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Play a notification sound when the user finishes interacting with the slider
            // This mimics the Windows Volume Mixer behavior
            BeepService.PlayBeep(BeepType.Information);
        }

        private void VolumeSlider_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left || 
                e.Key == System.Windows.Input.Key.Right || 
                e.Key == System.Windows.Input.Key.Up || 
                e.Key == System.Windows.Input.Key.Down || 
                e.Key == System.Windows.Input.Key.PageUp || 
                e.Key == System.Windows.Input.Key.PageDown)
            {
                BeepService.PlayBeep(BeepType.Information);
            }
        }
    }
}