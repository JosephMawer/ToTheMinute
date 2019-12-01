using HoursTracker.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HoursTracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TimeSheetViewModel TimeSheetViewModel;
       
        public MainPage()
        {
            this.InitializeComponent();
            TimeSheetViewModel = new TimeSheetViewModel();
        }
    }
}
