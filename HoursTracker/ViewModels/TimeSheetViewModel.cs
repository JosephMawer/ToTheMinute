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

        public ObservableCollection<Week> Week { get; set; }

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
            //TimeSheets = Db.GetTimeSheets().Result as ObservableCollection<TimeSheet>;

        }


        public async Task<List<TimeSheet>> GetTimeSheets() // => Db.GetTimeSheets();
        {
            // ** currently just here for testing this method
            TimeSheets = await Db.GetTimeSheets();





            //var TimeSheets = new List<TimeSheet>();
            //var weekArray = new ObservableCollection<Week>
            //{
            //    new Week() {Day = "Mon", TotalHours =7.5f},
            //    new Week() {Day = "Tue", TotalHours =7.5f},
            //    new Week() {Day = "Wed", TotalHours =7.5f},
            //    new Week() {Day = "Thu", TotalHours =7.5f},
            //    new Week() {Day = "Fri", TotalHours =7.5f},
            //    new Week() {Day = "Sat", TotalHours =0f},
            //    new Week() {Day = "Sun", TotalHours =0f}

            //};

            //TimeSheets.Add(new TimeSheet { Category = "Work", ClockedIn = true, Week = weekArray });
            //TimeSheets.Add(new TimeSheet { Category = "Music", ClockedIn = false, Week = weekArray });
            //TimeSheets.Add(new TimeSheet { Category = "Other", ClockedIn = false, Week = weekArray });

            return TimeSheets;
        }




    }
}
