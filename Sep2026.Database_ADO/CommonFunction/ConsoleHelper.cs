using System;

namespace Sep2026.Database_ADO.CommonFunction;

public static class ConsoleHelper
{
    // Keep asking until the user enters text.
    public static string ReadText(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (input == null)
            {
                throw new EndOfStreamException();
            }
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            Console.WriteLine("Please enter a value.");
        }
    }

    // TryParse checks the number without crashing on text such as "abc".
    public static int ReadNumber(string message)
    {
        while (true)
        {
            string input = ReadText(message);
            int number;
            bool isNumber = int.TryParse(input, out number);
            if (isNumber)
            {
                return number;
            }
            Console.WriteLine("Please enter a whole number.");
        }
    }

    // Display error message

    public static void ShowErrorMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    // Display success message
    public static void ShowSuccessMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    // Diaplay info message
    public static void ShowInfoMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }  
}
