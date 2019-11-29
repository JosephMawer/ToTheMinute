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
    }

    public class TimeSheetViewModel
    {
        public ObservableCollection<TimeSheet> TimeSheets  {get;set;}
        public TimeSheetViewModel()
        {
            TimeSheets = Db.GetTimeSheets().Result as ObservableCollection<TimeSheet>;

        }
        




    }
}
