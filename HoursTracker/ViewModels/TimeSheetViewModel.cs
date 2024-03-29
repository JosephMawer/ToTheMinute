﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace HoursTracker.ViewModels
{
    public class TimeSheet
    {
        public bool ClockedIn { get; set; }

        public string Category { get; set; }
         
        public List<Week> Week { get; set; }

        //public float TotalHoursForWeek { get; set; }
        public ICommand DummyCommand { get; set; }

        public TimeSheet()
        {
            DummyCommand = new RelayCommand(() =>
            {
                var action = ClockedIn ? TimeSheetService.ClockAction.ClockOut : TimeSheetService.ClockAction.ClockIn;
                TimeSheetService.AddData(action, Category).ConfigureAwait(false);
                 OnClockEvent(Category);
            });
        }

        public event EventHandler ClockEvent;

        private void OnClockEvent(string category)
        {
            ClockEvent?.Invoke(this, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public struct Week
    {
        public string Day { get; set; }
        public float TotalHours { get; set; }
    }

    public class TimeSheetViewModel : NotificationBase
    {
        public string CurrentDate { get; set; }
        public TimeSheet TimeSheet { get; set; }
        private string _weekday;

        public string Weekday
        {
            get => _weekday;
            set
            {
                _weekday = value;
                SetProperty(Weekday, value, () => Weekday = value);

                RaisePropertyChanged(nameof(Weekday));
            }
        }
        public string Date { get; set; }
        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }
        private readonly Timer Clock;

        private ObservableCollection<TimeSheet> _timeSheets;
        public ObservableCollection<TimeSheet> TimeSheets// { get; set; }
        {
            get => _timeSheets;
            set
            {
                _timeSheets = value;
                SetProperty(ref _timeSheets, value);

                RaisePropertyChanged(nameof(TimeSheets));
            }
        }


        public ICommand AddCategoryCommand { get; set; }

        public TimeSheetViewModel()
        {
            Clock = new Timer(ClockEvent, null,1000,1000);
            var time = DateTime.Now;
            Weekday = time.DayOfWeek.ToString();
            Date = time.ToString("MMMM dd, yyyy");
            Time = time.ToString("h:mm tt");
            AddCategoryCommand = new RelayCommand(() =>
            {
                TimeSheetService.AddData(TimeSheetService.ClockAction.ClockOut, "New Category").Wait();
                UpdateTimeSheet();
            });
            UpdateTimeSheet();
        }

        // this event handler is executed on a different thread
        private async void ClockEvent(object state)
        {
            //await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            //{
            //    var time = DateTime.Now;
            //    //if (time.Minute != this.Time)
            //    Weekday = time.DayOfWeek.ToString();
            //    Date = time.ToString("MM dd, yyyy");
            //    Time = time.ToString("h:mm tt");
            //});
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var time = DateTime.Now;
                //if (time.Minute != this.Time)
                Weekday = time.DayOfWeek.ToString();
                Date = time.ToString("MM dd, yyyy");
                Time = time.ToString("h:mm tt");
            });

        }

        public void UpdateTimeSheet()
        {
            TimeSheets = TimeSheetService.GetTimeSheets().Result;
            foreach (var timesheet in TimeSheets)
            {
                timesheet.ClockEvent += OnClockEventClicked;
            }
        }

        public void OnClockEventClicked(object sender, EventArgs e)
        {
            UpdateTimeSheet();
        }
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality
    /// to other objects by invoking delegates.
    /// The default return value for the CanExecute method is 'true'.
    /// RaiseCanExecuteChanged needs to be called whenever
    /// CanExecute is expected to return a different value.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }
        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// Determines whether this RelayCommand can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }
        /// <summary>
        /// Executes the RelayCommand on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            _execute();
        }
        /// <summary>
        /// Method used to raise the CanExecuteChanged event
        /// to indicate that the return value of the CanExecute
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
