using System.Windows;

namespace MultiTuner.View.ListViewControl
{
    /// <summary>
    /// Interaction logic for SongItemControl.xaml
    /// </summary>
    public partial class SongItemControl : BaseListControl
    {
        public SongItemControl()
        {
            InitializeComponent();
            // Initialize with vertical gap animation and "song" drop group
            InitializeListBox(SongList, enableDragDrop: true,
                gapOrientation: GapOrientation.Vertical,
                dropGroup: "song");
        }


        // The PreviewMouseDown handler is now inherited from BaseListControl
        // Just wire it up in XAML: PreviewMouseDown="ListBox_PreviewMouseDown"
        private void SongList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Call the base implementation
            ListBox_PreviewMouseDown(sender, e);
        }
    }
}