using System.Windows;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ビューモデルを設定する。
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }

        //! ビューモデルを保持する。
        private MainWindowViewModel ViewModel;
    }
}
