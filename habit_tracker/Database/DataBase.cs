
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;

using Habit;
namespace HabitTracker
{
    public class Database
    {
        private static string dbFile = "./habit_tracker.db";
        private static string connectionString = $"Data Source={dbFile}";


        public static void CreateDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {

                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                HabitType TEXT,
                Quantity INTEGER
                )";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void Update(int id, string newDate, string newHabit, int newQty)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (var updateCmd = connection.CreateCommand())
                    {
                        updateCmd.CommandText =
                            "UPDATE drinking_water SET date = @date, HabitType = @habit, Quantity = @qty WHERE Id = @id";

                        updateCmd.Parameters.AddWithValue("@date", newDate);
                        updateCmd.Parameters.AddWithValue("@habit", newHabit);
                        updateCmd.Parameters.AddWithValue("@qty", newQty);
                        updateCmd.Parameters.AddWithValue("@id", id);

                        updateCmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Record updated successfully.");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }
        }

        public static void Insert(string date, string habit, int quantity)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO drinking_water(Date, HabitType, Quantity) VALUES(@date, @habit, @quantity)";
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@habit", habit);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM drinking_water WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                int rowCount = cmd.ExecuteNonQuery();
                return rowCount > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return false;
            }
        }

        public static bool IfExist(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var checkCmd = connection.CreateCommand())
                {
                    checkCmd.CommandText = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @id)";
                    checkCmd.Parameters.AddWithValue("@id", id);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    return exists == 1;
                }
            }
        }


        public static List<DrinkingWater> GetAllRecords()
        {
            var tableData = new List<DrinkingWater>();

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (var tableCmd = connection.CreateCommand())
                    {
                        tableCmd.CommandText = "SELECT * FROM drinking_water";
                        using (var reader = tableCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tableData.Add(new DrinkingWater
                                {
                                    Id = reader.GetInt32(0),
                                    Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", CultureInfo.InvariantCulture),
                                    HabitType = reader.GetString(2),
                                    Quantity = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
            }

            return tableData;
        }
    }
}

