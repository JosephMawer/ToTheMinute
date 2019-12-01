using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HoursTracker.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HoursTracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<TimeSheet> TimeSheets;
        private TimeSheetViewModel _timesheetVM = new TimeSheetViewModel();
       
        
        public MainPage()
        {
            this.InitializeComponent();
            TimeSheets = _timesheetVM.GetTimeSheets().Result;
        }

        private async void ClockInClicked(object sender, RoutedEventArgs e)
        {
            //var action = _clockedIn ? Db.ClockAction.ClockOut : Db.ClockAction.ClockIn;
            //await Db.AddData(action);
        }

      
    }
}
