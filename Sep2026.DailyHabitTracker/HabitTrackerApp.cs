using System.Globalization;
namespace Sep2026.DailyHabitTracker;
public class HabitTrackerApp
{
    private static DayOfWeek[] AllDays = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday];
    private HabitRepository habitRepo;
    private HabitService service;

    public HabitTrackerApp(HabitRepository repository)
    {
        habitRepo = repository;
        service = new HabitService(repository);
    }

    // Start up
    public void Start()
    {
        Console.WriteLine("Daily Habit Tracker");
        while (true)
        {
            Console.WriteLine("\n1. Today's habits");
            Console.WriteLine("2. Create habit");
            Console.WriteLine("3. Edit habit");
            Console.WriteLine("4. Archive/unarchive");
            Console.WriteLine("5. Check in");
            Console.WriteLine("6. Undo check-in");
            Console.WriteLine("7. View all habits");
            Console.WriteLine("8. View history");
            Console.WriteLine("9. View statistics");
            Console.WriteLine("0. Exit");

            Console.Write("Choose a menu: ");
            try
            {
                switch (Console.ReadLine()?.Trim())
                {
                    case "1": 
                        ShowToday(); 
                        break;
                    case "2": 
                        Console.WriteLine("\n----- Create New Habit -----");
                        CreateHabit(); 
                        break;
                    case "3": 
                        Console.WriteLine("\n----- Edit Habit -----");
                        EditHabit(); 
                        break;
                    case "4": 
                        Console.WriteLine("\n----- Archive / Unarchive Habit -----");
                        ToggleArchive(); 
                        break;
                    case "5": 
                        Console.WriteLine("\n----- Check In Habit -----");
                        CheckIn(); 
                        break;
                    case "6": 
                        Console.WriteLine("\n----- Undo Check-In -----");
                        UndoCheckIn();
                        break;
                    case "7":
                        ViewAllHabits();
                        break;
                    case "8": 
                        Console.WriteLine("\n----- View Habit History -----");
                        ViewHistory(); 
                        break;
                    case "9": 
                        Console.WriteLine("\n----- View Habit Statistics -----");
                        ViewStatistics(); 
                        break;
                    case "0": 
                        ShowSuccessMessage("Program exited successfully.");
                        return;
                    default:
                        ShowErrorMessage("Please choose a number from 0 to 9.");
                        break;
                }
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }

            Console.WriteLine();
            Console.WriteLine("=========================================");
        }
    }

    private void ShowToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var habits = new List<HabitDto>();
        foreach (var habit in HabitService.habits)
        {
            if (!habit.IsArchived && habit.IsScheduledOn(today))
            {
                habits.Add(habit);
            }
        }

        Console.WriteLine("\nToday's Progress");
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine($"{"Habit",-25}Status");
        Console.WriteLine("--------------------------------------------");

        var completedCount = 0;
        foreach (var habit in habits)
        {
            var isCompleted = habit.CheckIns.Contains(today);
            var status = "○ Pending";
            if (isCompleted)
            {
                status = "✓ Completed";
                completedCount++;
            }

            Console.WriteLine($"{TruncateData(habit.Name, 24),-25}{status}");
        }

        var percentage = 0;
        if (habits.Count > 0)
        {
            percentage = completedCount * 100 / habits.Count;
        }

        Console.WriteLine();
        Console.WriteLine($"Progress: {completedCount} / {habits.Count} ({percentage}%)");
    }

    private void CreateHabit()
    {
        Console.Write("Name: ");
        var name = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowErrorMessage("Please enter habit name.");
            return;
        }

        Console.Write("Description (optional): ");
        var description = Console.ReadLine() ?? "";

        var days = ReadSchedule();
        if (days.Count == 0)
        {
            ShowErrorMessage("Please select at least one scheduled day.");
            return;
        }
        var habit = service.CreateNewHabit(name, description, days);
        ShowSuccessMessage("Created Successfully.");
    }

    private void EditHabit()
    {
        var habit = SelectHabit(true);
        if (habit is null)
        {
            return;
        }

        Console.Write($"Name [{habit.Name}]: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowErrorMessage("Please enter name.");
            return;
        }
        
        Console.Write($"Description [{habit.Description}]: ");
        var description = Console.ReadLine();

        Console.Write("Change schedule? (y/N): ");
        var days = habit.ScheduledDays;
        if (IsValueYes(Console.ReadLine()))
        {
            days = ReadSchedule();
        }

        if (days.Count == 0)
        {
            ShowErrorMessage("Please select at least one scheduled day.");
            return;
        }

        service.EditExistingHabit(habit, name, description ?? string.Empty, days);
        ShowSuccessMessage("Updated Successfully.");
    }

    private void ToggleArchive()
    {
        var habit = SelectHabit(true);
        if (habit is null)
        {
            return;
        }

        service.SetArchived(habit, !habit.IsArchived);
        if (habit.IsArchived)
        {
            ShowSuccessMessage("Archived Successfully.");
        }
        else
        {
            ShowSuccessMessage("Restored Successfully.");
        }
    }

    private void CheckIn()
    {
        var habit = SelectHabit(false);
        if (habit is null)
        {
            return;
        }

        var date = ReadDate("Check-in date", DateOnly.FromDateTime(DateTime.Today));
        if (service.CheckInHabit(habit, date))
        {
            ShowSuccessMessage("Checked in Successfully.");
        }
        else
        {
            ShowErrorMessage("Already checked in.");
        }
    }

    private void UndoCheckIn()
    {
        var habit = SelectHabit(true);
        if (habit is null)
        {
            return;
        }

        ShowPreviousCheckIns(habit);

        var date = ReadDate("Date to undo", DateOnly.FromDateTime(DateTime.Today));
        if (service.RemoveCheckIn(habit, date))
        {
            ShowSuccessMessage("Undo Successfully.");
        }
        else
        {
            ShowErrorMessage("No check-in existed on that date.");
        }
    }

    private void ShowPreviousCheckIns(HabitDto habit)
    {
        Console.WriteLine();
        Console.WriteLine("Previous");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"{"Date",-15}Status");
        Console.WriteLine("-----------------------------------------");

        var dates = habit.CheckIns.ToList();
        dates.Sort();
        dates.Reverse();

        foreach (var date in dates)
        {
            Console.WriteLine($"{date:yyyy-MM-dd}     ✓ Completed");
        }

        Console.WriteLine("-----------------------------------------");
    }

    private void ViewHistory()
    {
        var habit = SelectHabit(true);
        if (habit is null)
        {
            return;
        }

        Console.WriteLine($"\nCompleted history for {habit.Name}");
        if (habit.CheckIns.Count == 0)
        {
            ShowInfoMessage("No check-ins yet.");
            return;
        }

        var dates = habit.CheckIns.ToList();
        dates.Sort();
        foreach (var date in dates)
        {
            Console.WriteLine($" {date:yyyy-MM-dd} ({date.DayOfWeek})");
        }
    }

    private void ViewStatistics()
    {
        var habit = SelectHabit(true);
        if (habit is null)
        {
            return;
        }
        var stats = service.GetHabitStatistics(habit, DateOnly.FromDateTime(DateTime.Today));
        Console.WriteLine($"\n{habit.Name}\nSchedule: {FormatSchedule(habit.ScheduledDays)}");
        Console.WriteLine($"Completed: {stats.Completed}/{stats.Scheduled}\nCompletion: {stats.CompletionPercentage:F1}%");
        Console.WriteLine($"Current streak: {stats.CurrentStreak}\nLongest streak: {stats.LongestStreak}");
    }

    private void ViewAllHabits()
    {
        Console.WriteLine();
        Console.WriteLine("All Habits");
        Console.WriteLine("--------------------------------------------------------------------------");
        Console.WriteLine($"{"Name",-18}{"Description",-25}{"Schedule",-18}Created Date");
        Console.WriteLine("--------------------------------------------------------------------------");

        if (HabitService.habits.Count == 0)
        {
            ShowInfoMessage("There is no data.");
            return;
        }

        foreach (var habit in HabitService.habits)
        {
            var name = TruncateData(habit.Name, 17);
            var description = TruncateData(habit.Description, 24);
            var schedule = TruncateData(FormatSchedule(habit.ScheduledDays), 17);

            if (string.IsNullOrWhiteSpace(description))
            {
                description = "-";
            }

            Console.WriteLine($"{name,-18}{description,-25}{schedule,-18}{habit.CreatedOn:yyyy-MM-dd}");
        }
    }

    private HabitDto? SelectHabit(bool includeArchived)
    {
        var habits = new List<HabitDto>();
        foreach (var habit in HabitService.habits)
        {
            if (includeArchived || !habit.IsArchived)
            {
                habits.Add(habit);
            }
        }

        if (habits.Count == 0)
        {
            ShowInfoMessage("There is no data.");
            return null;
        }

        Console.WriteLine();
        foreach (var habit in habits)
        {
            Console.WriteLine($"{habit.Id}. {habit.Name} ({FormatSchedule(habit.ScheduledDays)}){(habit.IsArchived ? " [archived]" : "")}");
        }

        Console.Write("Select: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            ShowErrorMessage("Habit not found.");
            return null;
        }

        foreach (var habit in habits)
        {
            if (habit.Id == id)
            {
                return habit;
            }
        }

        return null;
    }

    private HashSet<DayOfWeek> ReadSchedule()
    {
        Console.Write("Schedule: D = daily, or 1 = Mon ... 7 = Sun (example 1,3,5): ");
        var input = (Console.ReadLine() ?? "").Trim();
        if (input.Equals("D", StringComparison.OrdinalIgnoreCase))
        {
            return AllDays.ToHashSet();
        }

        var days = new HashSet<DayOfWeek>();
        foreach (var part in input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(part, out var number) && number is >= 1 and <= 7)
            {
                days.Add(AllDays[number - 1]);
            }
        }

        return days;
    }

    private DateOnly ReadDate(string label, DateOnly defaultDate)
    {
        Console.Write($"{label} [{defaultDate:yyyy-MM-dd}]: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultDate;
        }

        if (DateOnly.TryParseExact(input.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new ArgumentException("Please use date format yyyy-MM-dd.");
    }

    private string FormatSchedule(IEnumerable<DayOfWeek> days)
    {
        var set = days.ToHashSet();
        if (set.Count == 7)
        {
            return "Daily";
        }

        var names = new List<string>();
        foreach (var day in AllDays)
        {
            if (set.Contains(day))
            {
                names.Add(day.ToString()[..3]);
            }
        }

        return string.Join(", ", names);
    }

    private bool IsValueYes(string? value)
    {
        if (value is null)
        {
            return false;
        }

        return value.Trim().Equals("y", StringComparison.OrdinalIgnoreCase);
    }

    private string TruncateData(string value, int maximumLength)
    {
        if (value.Length <= maximumLength)
        {
            return value;
        }

        return value[..maximumLength];
    }

    private void ShowErrorMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Message: {message}");
        Console.ForegroundColor = originalColor;
    }

    private void ShowSuccessMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    private void ShowInfoMessage(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }   
}
