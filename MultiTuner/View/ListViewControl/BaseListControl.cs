namespace MultiTuner.View.ListViewControl
{
    using GongSolutions.Wpf.DragDrop;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System;

    /// <summary>
    /// Gap orientation for drag-drop animations
    /// </summary>
    public enum GapOrientation
    {
        Vertical,   // Gap appears above/below (for vertical lists)
        Horizontal  // Gap appears left/right (for horizontal/wrap layouts)
    }

    /// <summary>
    /// Base class for ListBox-based user controls with drag-drop support
    /// </summary>
    public abstract partial class BaseListControl : UserControl
    {
        private Brush _originalBackground;
        private static BaseListControl _currentHoveredControl;
        private ListBoxItem _lastTargetItem;
        private bool _isSpaceAbove;
        private GapOrientation _gapOrientation = GapOrientation.Vertical;
        private string _dropGroup;
        private bool _isFixingSelection;


        protected ListBox ListBoxControl { get; private set; }

        protected BaseListControl()
        {
        }

        /// <summary>
        /// Initialize the ListBox control and set up drag-drop
        /// Call this from derived class constructor after InitializeComponent()
        /// </summary>
        /// <param name="listBox">The ListBox control to manage</param>
        /// <param name="enableDragDrop">Enable drag-drop functionality</param>
        /// <param name="gapOrientation">Direction of gap animation (Vertical or Horizontal)</param>
        /// <param name="dropGroup">Drop group identifier - only items with same group can be dropped</param>
        protected void InitializeListBox(
        ListBox listBox,
        bool enableDragDrop = true,
        GapOrientation gapOrientation = GapOrientation.Vertical,
        string dropGroup = null)
        {
            ListBoxControl = listBox;
            _originalBackground = ListBoxControl.Background;
            _gapOrientation = gapOrientation;
            _dropGroup = dropGroup;

            ListBoxControl.SelectionChanged += (s, e) =>
            {
                EnforceSingleSelection(ListBoxControl);
            };

            if (enableDragDrop)
            {
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(
                    ListBoxControl,
                    new BaseListDropHandler(this));
            }
        }


        #region Dependency Properties

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                nameof(Items),
                typeof(IEnumerable),
                typeof(BaseListControl),
                new PropertyMetadata(null));

        #endregion


        #region Single Selection Enforcement

        protected void EnforceSingleSelection(ListBox listBox)
        {
            if (_isFixingSelection)
                return;

            if (listBox.SelectedItems.Count <= 1)
                return;

            _isFixingSelection = true;

            ListBoxItem leftMostContainer = null;
            double minX = double.MaxValue;

            foreach (var item in listBox.SelectedItems)
            {
                var container = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item);
                if (container == null) continue;

                var position = container.TransformToAncestor(listBox)
                                        .Transform(new Point(0, 0));

                if (position.X < minX)
                {
                    minX = position.X;
                    leftMostContainer = container;
                }
            }

            listBox.SelectedItems.Clear();

            if (leftMostContainer != null)
                leftMostContainer.IsSelected = true;

            _isFixingSelection = false;
        }

        #endregion

        #region Mouse Handling

        /// <summary>
        /// Handles clicking on empty space to clear selection
        /// Wire this up in XAML: PreviewMouseDown="ListBox_PreviewMouseDown"
        /// </summary>
        protected void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var clickedItem = ItemsControl.ContainerFromElement(
                listBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (clickedItem == null)
            {
                listBox.SelectedItem = null;
            }
        }

        #endregion

        #region Drag-Drop Implementation

        private class BaseListDropHandler : IDropTarget
        {
            private readonly BaseListControl _parent;

            public BaseListDropHandler(BaseListControl parent)
            {
                _parent = parent;
            }

            public void DragOver(IDropInfo dropInfo)
            {
                // Check if source and target are compatible (same drop group)
                var sourceControl = FindParentControl(dropInfo.DragInfo?.VisualSource);
                var targetControl = _parent;

                // If drop groups are specified, ensure they match
                if (!string.IsNullOrEmpty(_parent._dropGroup) &&
                    sourceControl != null &&
                    !string.IsNullOrEmpty(sourceControl._dropGroup))
                {
                    if (sourceControl._dropGroup != targetControl._dropGroup)
                    {
                        // Incompatible drop groups - reject the drop and reset any animations
                        if (_currentHoveredControl != null)
                        {
                            _currentHoveredControl.ListBoxControl.Background = _currentHoveredControl._originalBackground;
                            _currentHoveredControl.ResetItemSpacing();
                            _currentHoveredControl = null;
                        }
                        dropInfo.Effects = DragDropEffects.None;
                        return;
                    }
                }

                // Reset the previous hovered control if it's different
                if (_currentHoveredControl != null && _currentHoveredControl != _parent)
                {
                    _currentHoveredControl.ListBoxControl.Background = _currentHoveredControl._originalBackground;
                    _currentHoveredControl.ResetItemSpacing();
                }

                // Change the ENTIRE ListBox background for the current one
                _parent.ListBoxControl.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x66, 0xFF));
                _currentHoveredControl = _parent;

                // Set drop effect
                dropInfo.Effects = DragDropEffects.Move;

                // Create gap animation
                if (dropInfo.VisualTargetItem is ListBoxItem targetItem)
                {
                    // Determine if we're inserting above or below
                    var insertIndex = dropInfo.InsertIndex;
                    var targetIndex = _parent.ListBoxControl.ItemContainerGenerator.IndexFromContainer(targetItem);
                    bool isAbove = insertIndex <= targetIndex;

                    _parent.CreateGapBetweenItems(targetItem, isAbove);
                }
            }

            public void Drop(IDropInfo dropInfo)
            {
                // Check compatibility again before dropping
                var sourceControl = FindParentControl(dropInfo.DragInfo?.VisualSource);
                var targetControl = _parent;

                if (!string.IsNullOrEmpty(_parent._dropGroup) &&
                    sourceControl != null &&
                    !string.IsNullOrEmpty(sourceControl._dropGroup))
                {
                    if (sourceControl._dropGroup != targetControl._dropGroup)
                    {
                        // Incompatible - cancel drop
                        _parent.ResetItemSpacing();
                        _parent.ListBoxControl.Background = _parent._originalBackground;
                        _currentHoveredControl = null;
                        return;
                    }
                }

                // Reset everything
                _parent.ResetItemSpacing();
                _parent.ListBoxControl.Background = _parent._originalBackground;
                _currentHoveredControl = null;

                // Use default drop behavior
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

            private BaseListControl FindParentControl(DependencyObject element)
            {
                while (element != null)
                {
                    if (element is BaseListControl control)
                        return control;
                    element = VisualTreeHelper.GetParent(element);
                }
                return null;
            }
        }

        private void CreateGapBetweenItems(ListBoxItem targetItem, bool isAbove)
        {
            double duration = 450;

            // Reset previous item if different
            if (_lastTargetItem != null && _lastTargetItem != targetItem)
            {
                AnimateMargin(_lastTargetItem, new Thickness(0), duration);
            }
            else if (_lastTargetItem == targetItem && _isSpaceAbove != isAbove)
            {
                // Same item but different side, reset first to prevent "snapping"
                AnimateMargin(_lastTargetItem, new Thickness(0), duration);
            }

            _lastTargetItem = targetItem;
            _isSpaceAbove = isAbove;

            // Create gap based on orientation (60px)
            Thickness newMargin;
            if (_gapOrientation == GapOrientation.Vertical)
            {
                // Vertical: gap above or below
                newMargin = isAbove ? new Thickness(0, 60, 0, 0) : new Thickness(0, 0, 0, 60);
            }
            else
            {
                // Horizontal: gap left or right
                newMargin = isAbove ? new Thickness(60, 0, 0, 0) : new Thickness(0, 0, 60, 0);
            }

            AnimateMargin(targetItem, newMargin, duration);
        }

        private void ResetItemSpacing()
        {
            if (_lastTargetItem != null)
            {
                AnimateMargin(_lastTargetItem, new Thickness(0), 450);
                _lastTargetItem = null;
            }
        }

        private void AnimateMargin(ListBoxItem item, Thickness to, double durationMs)
        {
            var animation = new ThicknessAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };

            item.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        #endregion
    }
}