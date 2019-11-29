using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            _clockedIn = !_clockedIn;
            ClockActionButton.Content = _clockedIn ? "Clock Out" : "Clock In";
            _localSettings.Values["ClockedIn"] = _clockedIn;
        }
    }
}
