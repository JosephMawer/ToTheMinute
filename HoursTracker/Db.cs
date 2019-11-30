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
    public class Db
    {

        private static readonly string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");

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

        public static async Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);

            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();

                foreach (var table in TablesArray)
                {
                    using (var cmd = new SqliteCommand(table, db))
                    {
                        await cmd.ExecuteReaderAsync();
                    }
                }
                
            }
        }
        public enum ClockAction
        {
            ClockIn,
            ClockOut
        }

        // todo: ensure category is sent when adding data
        public static async Task AddData(ClockAction action)
        {
            using (var con = new SqliteConnection($"Filename={dbpath}"))
            {
                await con.OpenAsync();

                using (var cmd = new SqliteCommand())
                {
                    cmd.Connection = con;

                    // Use parameterized query to prevent SQL injection attacks
                    cmd.CommandText = $"insert into {Table} ('Action', 'TimeOfAction') values (@action, @time)";
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@time", DateTimeSQLite(DateTime.Now));

                    await cmd.ExecuteReaderAsync();
                }
            }
        }

        // returns a list of distinct categories from database
        public static async Task<List<string>> GetDistinctCategories()
        {
            using (var con = new SqliteConnection($"Filename={dbpath}"))
            {
                await con.OpenAsync();

                using (var cmd = new SqliteCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = $"select distinct(Category) from {Table}";
                    var categories = new List<string>();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        // Add category to return list if it is not null or empty
                        if (!string.IsNullOrEmpty(reader.SafeGetString(0)))
                            categories.Add(reader.GetString(0));
                    }

                    return categories;
                }
            }
        }

        public static async Task<List<TimeSheet>> GetTimeSheets()
        {
            var timeSheets = new List<TimeSheet>();
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
                    Week = await GetWeeklyData(dt)
                };
                timeSheets.Add(ts);
            }

            return timeSheets;
        }

        private static async Task<ObservableCollection<Week>> GetWeeklyData(List<POCO> table)
        {
            var weeklyData = new ObservableCollection<Week>();
            var (startOfWeek, endOfWeek) = GetCurrentWeek();
            var startDate = DateTime.Parse(startOfWeek);
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                var data = table.Where(x => x.TimeOfAction.Date == currentDate.Date).ToList();
               
                // start adding hours for each day
                double runningTotal = 0;
                for (int j = 1; j < data.Count; j += 2)
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

        // converts data returned from query into a timesheet object
        private static async Task<TimeSheet> GetTimeSheetFromSql()
        {
            throw new NotImplementedException();
        }


        public static async Task<List<POCO>> GetList(string sql)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqliteConnection($"Filename={dbpath}"))
                {
                    await con.OpenAsync();
                    using (var cmd = new SqliteCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = sql;
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
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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

//select* from ClockTransactions
//where TimeOfAction between '2019-11-25 0:0:0' and '2019-12-02 0:0:0'
//and Category = 'school'