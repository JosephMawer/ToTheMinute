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
        public bool ClockedIn { get; set; }

        public string Category { get; set; }

        public List<Week> Week { get; set; }

        //public float TotalHoursForWeek { get; set; }
    }

    public struct Week
    {
        public string Day { get; set; }
        public float TotalHours { get; set; }
    }

    public class TimeSheetViewModel
    {
        private List<TimeSheet> _timeSheets;
        public List<TimeSheet> TimeSheets
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

        }

        // returns a list of time sheet objects
        public async Task<List<TimeSheet>> GetTimeSheets() => await Db.GetTimeSheets();





    }
}
