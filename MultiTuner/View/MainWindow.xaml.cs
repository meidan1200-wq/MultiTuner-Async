using System.Windows;
using System.Windows.Input;
using MultiTuner.ViewModel;


namespace MultiTuner
{

    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // Fix: Ensure the window respects the taskbar area
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            this.StateChanged += MainWindow_StateChanged;
        }


        // 1. Logic for dragging the window and double-click to maximize
        private void Toolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    ToggleMaximize();
                }
                else
                {
                    this.DragMove();
                }
            }
        }

        // 2. Button Click Handlers (Required for the XAML Click="...")
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // 3. Maximization Logic
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                // Add padding when maximized to avoid clipping
                MainWindowBorder.BorderThickness = new Thickness(6);
            }
            else
            {
                // Standard border when windowed
                MainWindowBorder.BorderThickness = new Thickness(1);
            }
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(this);
            else
                SystemCommands.MaximizeWindow(this);
        }





    }
}