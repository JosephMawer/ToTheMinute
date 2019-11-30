using Microsoft.Data.Sqlite;

namespace HoursTracker.Extensions
{
    public static class Extensions
    {
        public static string SafeGetString(this SqliteDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }
    }
}
