
using System;
using Microsoft.Data.Sqlite;

namespace habit_tracker{

    class Program
    {
        static void Main(string[] args)
        {
            string connectionString =(@"Data Source=habit_tracker.db");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                
                tableCmd = @



            }
        }
    }
}