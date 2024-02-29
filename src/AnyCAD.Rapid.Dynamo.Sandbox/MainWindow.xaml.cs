using MahApps.Metro.Controls;

namespace AnyCAD.Rapid.Dynamo.Sandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel(mRenderCtrl);
            this.DataContext = viewModel;
        }

        public MainViewModel ViewModel { get => (MainViewModel)this.DataContext; }

        private void mRenderCtrl_ViewerReady()
        {
            ViewModel.Initialize();
        }
    }
}