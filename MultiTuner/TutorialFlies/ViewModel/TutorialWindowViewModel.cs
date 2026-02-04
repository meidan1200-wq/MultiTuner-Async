using MultiTuner.Model;
using System.Collections.ObjectModel;
using MultiTuner.ViewModel;
using MultiTuner.ViewModel.TestUiViewModel;

namespace MultiTuner.TutorialFlies.ViewModel
{
    //internal class TutorialWindowViewModel : ViewModelBase
    //{
    //    public ObservableCollection<DataBaseItem> Items { get; set; }

    //    public RelayCommand AddCommand => new RelayCommand(execute => Additem());
    //    public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => selectedItem != null);
    //    public RelayCommand SaveCommand => new RelayCommand(execute => Save(), canExecute => CanSave());


    //    public TutorialWindowViewModel()
    //    {
    //        Items = new ObservableCollection<DataBaseItem>();


    //        //Items.Add(new DataBaseItem 
    //        //{ 
    //        //    Name = "Product1",
    //        //    SerialNumber = "001", 
    //        //    Quantity = 10 
    //        //});
    //        //Items.Add(new DataBaseItem
    //        //{
    //        //    Name = "Product1",
    //        //    SerialNumber = "001",
    //        //    Quantity = 10
    //        //});
    //    }

    //    private DataBaseItem selectedItem;

    //    public DataBaseItem SelectedItem
    //    {
    //        get { return selectedItem; }
    //        set 
    //        {
    //            selectedItem = value;
    //            OnPropertyChanged();
              
    //        }
    //    }

    //    private void Additem()
    //    {
    //        Items.Add(new DataBaseItem
    //        {
    //            Name = "New Item",
    //            SerialNumber = "xxxx",
    //            Quantity = 0
    //        });
    //    }

    //    private void DeleteItem()
    //    {
    //       Items.Remove(SelectedItem);
    //    }

    //    private void Save()
    //    {
    //        // Save logic here
    //    }

    //    private bool CanSave() 
    //    {
    //        return true;

    //    }

    //}
}
