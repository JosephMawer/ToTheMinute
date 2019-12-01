using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Perception.Provider;
using Windows.Storage;
using Windows.UI.StartScreen;
using HoursTracker.ViewModels;
using Microsoft.Data.Sqlite;
using HoursTracker.Extensions;

namespace HoursTracker
{
    public class POCO
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public DateTime TimeOfAction { get; set; }
        public string Category { get; set; }
    }

    // C:\Users\joseph.mawer.IDEA\AppData\Local\Packages\c16400a1-8cf8-40b0-8ba7-fea371f99e4b_t0yaebqe3tq6a\LocalState
    public class TimeSheetService
    {
        private static SqliteConnection _connection;

        private const string Table = "ClockTransactions";
    
        private static readonly string CreateClockTransactions =
            $@"CREATE TABLE IF NOT EXISTS '{Table}' ('ID' INTEGER NOT NULL UNIQUE,'Action' INTEGER,'TimeOfAction' TEXT,'Category' TEXT, PRIMARY KEY('ID'))";

        // add new tables here..
        private static readonly string[] TablesArray = new[]{CreateClockTransactions};

        // datetime helper function
        public static string DateTimeSQLite(DateTime datetime)
        {
            string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}";
            return string.Format(dateTimeFormat, datetime.Year, datetime.Month.ToString("D2"), datetime.Day.ToString("D2"), datetime.Hour, datetime.Minute, datetime.Second);
        }

        // Opens the database connection and ensures all tables exist
        public static async Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");

            _connection = new SqliteConnection($"Filename={dbpath}");
            await _connection.OpenAsync();

            foreach (var table in TablesArray)
            {
                using (var cmd = new SqliteCommand(table, _connection)) {
                    await cmd.ExecuteReaderAsync();
                }
            }
        }

        public enum ClockAction
        {
            ClockIn,
            ClockOut
        }

        // todo: ensure category is sent when adding data
        public static async Task AddData(ClockAction action, string category)
        {
            using (var _command = new SqliteCommand())
            {
                _command.Connection = _connection;
                // Use parameterized query to prevent SQL injection attacks
                _command.CommandText = $"insert into {Table} ('Action', 'TimeOfAction', 'Category') values (@action, @time, @category)";
                _command.Parameters.AddWithValue("@action", action);
                _command.Parameters.AddWithValue("@time", DateTimeSQLite(DateTime.Now));
                _command.Parameters.AddWithValue("@category", category);
                await _command.ExecuteReaderAsync();
            }
        }

        // returns a list of distinct categories from database
        public static async Task<List<string>> GetDistinctCategories()
        {
            var sql = $"select distinct(Category) from {Table}";
            using (var _command = new SqliteCommand(sql,_connection))
            {
                var categories = new List<string>();
                var reader = await _command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    // Add category to return list if it is not null or empty
                    if (!string.IsNullOrEmpty(reader.SafeGetString(0)))
                    {
                        categories.Add(reader.GetString(0));
                    }
                }

                return categories;
            }
        }

        public static async Task<List<POCO>> GetList(string sql)
        {
            using (var cmd = new SqliteCommand(sql, _connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var pocoList = new List<POCO>();
                    while (reader.Read())
                    {
                        var poco = new POCO();
                        poco.ID = reader.GetInt32(0);
                        poco.Action = reader.GetString(1);
                        poco.TimeOfAction = DateTime.Parse(reader.GetString(2));
                        poco.Category = reader.GetString(3);
                        pocoList.Add(poco);
                    }

                    return pocoList;
                }
            }
        }

        // returns a list of time sheet objects
        public static async Task<ObservableCollection<TimeSheet>> GetTimeSheets()
        {
            var timeSheets = new ObservableCollection<TimeSheet>();
            var (startOfWeek, endOfWeek) = GetCurrentWeek();
            var sql = $"select * from {Table} where TimeOfAction between '{startOfWeek}' and '{endOfWeek}'";
            
            var categories = await GetDistinctCategories();
            foreach (var category in categories)
            {
                sql += $" and Category = '{category}'";
                var dt = await GetList(sql);
                var ts = new TimeSheet
                {
                    Category = category,
                    Week = GetWeeklyData(dt),
                    ClockedIn = await IsUserClockedIn(category)
                };
                timeSheets.Add(ts);
            }

            return timeSheets;
        }

        // helper function to determine if user is currently clocked in
        private static async Task<bool> IsUserClockedIn(string category)
        {
            var sql = $"select Action from ClockTransactions where Category = '{category}' order by ID desc limit 1";
            using (var cmd = new SqliteCommand(sql,_connection)) {
                return (await cmd.ExecuteScalarAsync() as string) == "0";
            }
        }

        // gets the 'week' portion of the time sheet, which should look something like this:
        // mon  | tue   | wed   | thu   
        // 7.5  | 8     | 6.5   | 7.5
        private static List<Week> GetWeeklyData(List<POCO> table)
        {
            var weeklyData = new List<Week>();
            var (startOfWeek, endOfWeek) = GetCurrentWeek();
            var startDate = DateTime.Parse(startOfWeek);
            for (var i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                var data = table.Where(x => x.TimeOfAction.Date == currentDate.Date).ToList();
               
                // start adding hours for each day of the week
                double runningTotal = 0;
                for (var j = 1; j < data.Count; j += 2)
                {
                    runningTotal += data.ElementAt(j).TimeOfAction.Subtract(data.ElementAt(j - 1).TimeOfAction)
                        .TotalMinutes;
                }

                var week = new Week();
                week.Day = ((DayOfWeek) i).ToString().Substring(0,3);
                week.TotalHours = (float)runningTotal;
                weeklyData.Add(week);
            }
            return weeklyData;
        }

        // helper function to return the start and end date of the week as string
        private static (string startOfWeek, string endOfWeek) GetCurrentWeek()
        {
            var result = (start: "", end: "");

            var startOfWeek = DateTime.Today;
            var delta = DayOfWeek.Monday - startOfWeek.DayOfWeek;
            startOfWeek = startOfWeek.AddDays(delta);
            var endOfWeek = startOfWeek.AddDays(7);

            result.start = DateTimeSQLite(startOfWeek);
            result.end = DateTimeSQLite(endOfWeek);

            return result;
        }
    }
}
