namespace MultiTuner.View.ListViewControl
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PlaylistItemControl.xaml
    /// </summary>
    public partial class PlaylistItemControl : BaseListControl
    {
        public PlaylistItemControl()
        {
            InitializeComponent();
            // Initialize with horizontal gap animation and "playlist" drop group
            InitializeListBox(PlaylistList, enableDragDrop: true,
                gapOrientation: GapOrientation.Horizontal,
                dropGroup: "playlist");

            //PlaylistList.SelectionChanged += PlaylistList_SelectionChanged;
        }

        // DependencyProperty for SelectedItem
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(PlaylistItemControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        //// Forward the ListBox selection to this property
        //private void PlaylistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    SelectedItem = PlaylistList.SelectedItem;
        //}




        // Scale property specific to PlaylistItemControl
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                nameof(Scale),
                typeof(double),
                typeof(PlaylistItemControl),
                new PropertyMetadata(1.0));

        // The PreviewMouseDown handler is now inherited from BaseListControl
        private void PlaylistList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Call the base implementation
            ListBox_PreviewMouseDown(sender, e);
        }
    }
}