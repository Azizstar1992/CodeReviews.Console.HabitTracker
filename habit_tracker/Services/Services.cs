
using HabitTracker;
using Habit;
using System.Globalization;

namespace Services
{
    public class Service
    {
        public static void GetUserInput()
        {
            Console.Clear();
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
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("not a valid command.Please choose from 0 to 4");
                        break;

                }
            }
        }

        internal static void GetAllRecords()
        {
            PrintAllRecords();
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
        internal static void PrintAllRecords()
        {
            Console.Clear();
            var records = Database.GetAllRecords();

            if (records.Count == 0)
            {
                Console.WriteLine("No Rows Found, Database is Empty");
                return;
            }

            Console.WriteLine("ID--Type--Date--Amount----------");
            Console.WriteLine("---------------------------------");

            foreach (var dw in records)
            {
                Console.WriteLine($"{dw.Id} - {dw.HabitType} - {dw.Date:dd-MM-yy} - Quantity: {dw.Quantity}");
            }

            Console.WriteLine("---------------------------------");
        }

        internal static void Update()
        {

            
            while (true)
            {
                PrintAllRecords();
                Console.WriteLine(
                    "Enter the ID you wish to Delete or Press x to return"
                );

                string? input = Console.ReadLine();
                if (input == null) continue;

                if (input.ToLower() == "x") return;

                if (!int.TryParse(input, out int recordId))
                {
                    Console.WriteLine("Invalid input. Please enter a numeric ID or 'x' to exit.");
                    continue;
                }

                if (!Database.IfExist(recordId))
                {
                    Console.WriteLine("Error Record not found");
                    continue;
                }

                string? date = GetDateInput();
                string habit = GetHabit();
                int qty = GetNumberInput("Enter new Quantity:");
                Database.Update(recordId, date, habit, qty);
            }

        }


        internal static void Insert()
        {
            bool success = false;

            while (!success)
            {
                string? date = GetDateInput();
                if (date == null) return; // user cancelled
                string habit = GetHabit();
                int quantity = GetNumberInput("\nPlease insert the number of glasses or other measure:");

                try
                {
                    Database.Insert(date, habit, quantity);
                    Console.WriteLine("Record inserted successfully!");
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to insert record: " + ex.Message);
                    Console.WriteLine("Do you want to retry? (Y/N)");
                    string? choice = Console.ReadLine();
                    if (choice == null || choice.Equals("N", StringComparison.OrdinalIgnoreCase))
                        return; // abort
                }
            }

            Console.WriteLine("press any key to continue");
            Console.ReadKey(true);
        }


        internal static void Delete()
        {
            PrintAllRecords();
            Console.WriteLine("Enter the ID of the record to delete:");
            int id = GetNumberInput("ID:"); // your input helper

            
            if (!Database.IfExist(id))
            {
                Console.WriteLine($"Record with ID {id} not found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }


            try
            {
                Database.Delete(id);
                Console.WriteLine($"Record with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete record: " + ex.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        internal static int GetNumberInput(string message)
        {
            int amount = 0;
            Console.WriteLine(message);

            string? numberInput = Console.ReadLine();
            numberInput = numberInput.Trim();
            while (numberInput == null || !int.TryParse(numberInput, out amount) || amount < 0)
            {
                Console.WriteLine("that was an incorrect number");
                numberInput = Console.ReadLine();
            }
            return amount;
        }
        internal static string GetDateInput()
        {
            Console.WriteLine("\nEnter the date (dd-MM-yy).");
            Console.WriteLine("Type 0 to return, or T to use today's date.");

            string? dateInput = Console.ReadLine();

            if (dateInput.Equals("T", StringComparison.OrdinalIgnoreCase))
                return DateTime.Now.ToString("dd-MM-yy");

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

        internal static string GetHabit()
        {
            Console.WriteLine("Please enter the type, Max 20 characters long");
            string? typeInput = Console.ReadLine();
            while (typeInput == null || typeInput.Length > 20)
            {
                Console.WriteLine("Please enter the type, Max 20 characters long");
                typeInput = Console.ReadLine();
            }

            return typeInput;
        }

    }
}