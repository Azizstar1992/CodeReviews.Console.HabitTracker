
using System;
using System.Globalization;
using System.Numerics;
using Microsoft.Data.Sqlite;

namespace habit_tracker
{

    public class DrinkingWater
    {
        public int Id { get; set; }
        public int quantity { get; set; }
        public DateTime Date { get; set; }
    }
    class Program
    {
        private static string dbFile;
        private static string connectionString;

        static void Main(string[] args)
        {
            dbFile = "./habit_tracker.db";
            Console.WriteLine("Database will be at: " + System.IO.Path.GetFullPath(dbFile));
            connectionString = ($"Data Source={dbFile}");
            using (var connection = new SqliteConnection(connectionString))
            {

                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
                )";

                tableCmd.ExecuteNonQuery();


                connection.Close();


            }
            GetUserInput();

        }

        static void GetUserInput()
        {

            bool closeApp = false;
            while (!closeApp)
            {
                Console.Clear();
                Console.WriteLine("\nMAIN MENU");
                Console.WriteLine("\nPlease Enter your Choice!");
                Console.WriteLine("\nType 0 to close Application");
                Console.WriteLine("Enter 1 to View All Records.");
                Console.WriteLine("Enter 2 to Insert Record.");
                Console.WriteLine("Enter 3 to Delete Record.");
                Console.WriteLine("Enter 4 to Update Record.");
                Console.WriteLine("-----------------------------------------\n");

                string? command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        closeApp = true;
                        break;
                    case "1":
                        getAllRecords();
                        break;
                    case "2":
                        insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        update();
                        break;
                    default:
                        Console.WriteLine("not a valid command.Please choose from 0 to 4");
                        break;

                }
            }
        }
        private static void insert()
        {
            string? date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease Insert a number of glasses or other measure of your choice (no decimals allowed)\n\n");
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO drinking_water(date,quantity) VALUES('{date}',{quantity})";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            int amount = 0;
            string? numberInput = Console.ReadLine();
            while (numberInput == null || !int.TryParse(numberInput, out amount) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("that was an incorrect number");
                numberInput = Console.ReadLine();
            }
            return amount;
        }
        internal static string? GetDateInput()
        {
            Console.WriteLine("\nEnter the date (dd-MM-yy). Type 0 to return.");

            string? dateInput = Console.ReadLine();


            if (dateInput == "0") return null;

            while (!DateTime.TryParseExact(
                dateInput,
                "dd-MM-yy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            ))
            {
                Console.WriteLine("Wrong format. Use dd-MM-yy.\nType 0 to return.");

                dateInput = Console.ReadLine();

                if (dateInput == "0")
                    return null;
            }

            return dateInput;
        }
        // printing function
        private static void printAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();
                SqliteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-GB")),
                                quantity = reader.GetInt32(2)
                            }
                        );
                    }
                }
                else
                {
                    Console.WriteLine("No Rows Found, DataBase is Empty");
                }
                connection.Close();
                Console.WriteLine("---------------------------------");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yy")} - Quantity: {dw.quantity} ");
                }
                Console.WriteLine("---------------------------------");
            }
        }
        private static void getAllRecords()
        {
            printAllRecords();
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }

        private static void Delete()
        {
            bool keepDeleting = true;

            while (keepDeleting)
            {
                printAllRecords();

                Console.WriteLine("Enter the ID of the habit you wish to delete, or type 'x' to return to the menu:");
                string? input = Console.ReadLine();

                if (input == null) continue;

                if (input.ToLower() == "x")
                {
                    keepDeleting = false;
                    break;
                }
                int recordId;
                if (!int.TryParse(input, out recordId))
                {
                    Console.WriteLine("Invalid input. Please enter a number or 'x' to exit.");
                    continue;
                }

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = $"DELETE FROM drinking_water WHERE id = {recordId}";

                    int rowCount = tableCmd.ExecuteNonQuery();

                    if (rowCount == 0)
                    {
                        Console.WriteLine($"ID {recordId} not found!\n");
                    }
                    else
                    {
                        Console.WriteLine($"Record with ID {recordId} has been deleted.\n");
                    }

                    connection.Close();
                }
                Console.WriteLine("press y to continue, any other key to return to menu");
                input = Console.ReadLine();

                if (input == null || input == "y") continue;
                else
                {
                    keepDeleting = false;
                    break;
                }


            }
        }

        private static void update()
        {
            while (true) // loop until user updates something or exits
            {
                printAllRecords();

                Console.WriteLine(
                    "Enter the ID of the record you want to update.\n" +
                    "Type 'x' to return to the menu."
                );

                string? input = Console.ReadLine();
                if (input == null) continue;

                if (input.ToLower() == "x") return; // exit safely


                int recordId;
                if (!int.TryParse(input, out recordId))
                {
                    Console.WriteLine("Invalid input. Please enter a numeric ID or 'x' to exit.");
                    continue;
                }

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();


                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @id)";
                    checkCmd.Parameters.AddWithValue("@id", recordId);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists == 0)
                    {
                        Console.WriteLine($"\nRecord with ID {recordId} does not exist.");
                        continue;
                    }


                    string? date = GetDateInput();
                    int qty = GetNumberInput("Enter new quantity:");


                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText =
    "UPDATE drinking_water SET date = @date, quantity = @qty WHERE Id = @id";

                    updateCmd.Parameters.AddWithValue("@date", date);
                    updateCmd.Parameters.AddWithValue("@qty", qty);
                    updateCmd.Parameters.AddWithValue("@id", recordId);
                    updateCmd.ExecuteNonQuery();
                    connection.Close();
                }

                Console.WriteLine("Record updated successfully.");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey(true);
                return;
            }
        }
    }
}
