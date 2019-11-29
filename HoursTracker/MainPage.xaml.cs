using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public MainPage()
        {
            this.InitializeComponent();

            _localSettings = ApplicationData.Current.LocalSettings;
            _clockedIn = _localSettings.Values["ClockedIn"] as bool? ?? false;
           
            Db.InitializeDatabase().ConfigureAwait(false);
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
            ClockActionButton.Content = _clockedIn ? "Clock Out" : "Clock In";
        }
    }
}
