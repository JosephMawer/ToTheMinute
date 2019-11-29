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
        private readonly ApplicationDataContainer _localSettings;
        private bool _clockedIn;
        private List<TimeSheet> TimeSheets;

        public MainPage()
        {
            this.InitializeComponent();
            TimeSheets = new List<TimeSheet>();
            
            var weekArray = new ObservableCollection<Week>
            {
                new Week() {Day = "Mon", TotalHours =7.5f},
                new Week() {Day = "Tue", TotalHours =7.5f},
                new Week() {Day = "Wed", TotalHours =7.5f},
                new Week() {Day = "Thu", TotalHours =7.5f},
                new Week() {Day = "Fri", TotalHours =7.5f},
                new Week() {Day = "Sat", TotalHours =0f},
                new Week() {Day = "Sun", TotalHours =0f}

            };

            TimeSheets.Add(new TimeSheet {Category = "Work", ClockedIn = true, Week = weekArray });
            TimeSheets.Add(new TimeSheet { Category = "Music", ClockedIn = false, Week = weekArray });
            TimeSheets.Add(new TimeSheet { Category = "Other", ClockedIn = false, Week = weekArray });

            
            _localSettings = ApplicationData.Current.LocalSettings;
            _clockedIn = _localSettings.Values["ClockedIn"] as bool? ?? false;
            // updates text on button
            //ClockActionButton.Content = _clockedIn ? "Clock Out" : "Clock In";
        }

        private async void ClockInClicked(object sender, RoutedEventArgs e)
        {
            var action = _clockedIn ? Db.ClockAction.ClockOut : Db.ClockAction.ClockIn;
            await Db.AddData(action);
            UpdateSettings();
        }

        // manages state, i.e. clocked-in or clocked-out
        private void UpdateSettings()
        {
            // updates internal state
            _clockedIn = !_clockedIn;
            _localSettings.Values["ClockedIn"] = _clockedIn;

            // updates text on button
            //ClockActionButton.Content = _clockedIn ? "Clock Out" : "Clock In";
        }
    }
}
