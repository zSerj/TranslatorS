using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Logics for MainWindow.xaml
    /// </summary>
    public partial class ClientView : Window
    {
        public ClientView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //((VM.VMtranslate)this.DataContext)
        }

    }
}
