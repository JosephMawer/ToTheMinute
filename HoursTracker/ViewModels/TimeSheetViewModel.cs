using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoursTracker.ViewModels
{
    public class TimeSheet
    {
        private bool _clockedIn;
        public bool ClockedIn
        {
            get => _clockedIn;
            set
            {
                _clockedIn = value;
                //OnProperyChangedEvent();
            }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                //OnPropertyChanged();
            }
        }

        public ObservableCollection<Week> Week { get; set; }

        public float TotalHoursForWeek { get; set; }
    }

    public struct Week
    {
        public string Day { get; set; }
        public float TotalHours { get; set; }
    }

    public class TimeSheetViewModel
    {
        private ObservableCollection<TimeSheet> _timeSheets;
        public ObservableCollection<TimeSheet> TimeSheets
        {
            get => _timeSheets;
            set
            {
                _timeSheets = value;
                //OnPropertyChanged();
            }
        }
        public TimeSheetViewModel()
        {
            //TimeSheets = Db.GetTimeSheets().Result as ObservableCollection<TimeSheet>;

        }
        




    }
}
