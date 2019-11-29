using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Data.Sqlite;

namespace HoursTracker
{
    // C:\Users\joseph.mawer.IDEA\AppData\Local\Packages\c16400a1-8cf8-40b0-8ba7-fea371f99e4b_t0yaebqe3tq6a\LocalState
    public class Db
    {

        private static readonly string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");

        private const string CreateClockTransactions =
            @"CREATE TABLE IF NOT EXISTS 'ClockTransactions' ('ID' INTEGER NOT NULL UNIQUE,'Action' INTEGER,'TimeOfAction' TEXT,PRIMARY KEY('ID'))";

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

        public static async Task AddData(ClockAction action)
        {
            using (var con = new SqliteConnection($"Filename={dbpath}"))
            {
                await con.OpenAsync();

                using (var cmd = new SqliteCommand())
                {
                    cmd.Connection = con;

                    // Use parameterized query to prevent SQL injection attacks
                    cmd.CommandText = "insert into ClockTransactions ('Action', 'TimeOfAction') values (@action, @time)";
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@time", DateTimeSQLite(DateTime.Now));

                    await cmd.ExecuteReaderAsync();
                }
            }
        }
    }
}
