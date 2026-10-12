namespace Sep2026.DailyHabitTracker;

public class HabitService
{
    public static List<HabitDto> habits = new();
    private HabitRepository repository;

    public HabitService(HabitRepository repository)
    {
        this.repository = repository;
        habits = repository.LoadFile();
    }

    // Create a new habit with given name, description, and scheduled days.
    public HabitDto CreateNewHabit(string name, string description, IEnumerable<DayOfWeek> scheduledDays)
    {
        var days = scheduledDays.ToHashSet();

        var habit = new HabitDto
        {
            Id = habits.Count > 0 ? habits.Max(h => h.Id) + 1 : 1,
            Name = name.Trim(),
            Description = description.Trim(),
            CreatedOn = DateOnly.FromDateTime(DateTime.Today),
            ScheduledDays = days
        };
        habits.Add(habit);
        SaveToFile();
        return habit;
    }

    // Edit existing habit.
    public void EditExistingHabit(HabitDto habit, string name, string description, IEnumerable<DayOfWeek> days)
    {
        var schedule = days.ToHashSet();
        habit.Name = name.Trim();
        habit.Description = description.Trim();
        habit.ScheduledDays = schedule;
        SaveToFile();
    }

    // Remove a habit from the list.
    public void SetArchived(HabitDto habit, bool archived)
    {
        habit.IsArchived = archived;
        SaveToFile();
    }

    // Mark complete status for a specific date.
    public bool CheckInHabit(HabitDto habit, DateOnly date)
    {
        if (!habit.IsScheduledOn(date))
        {
            throw new InvalidOperationException("Selected habit is not scheduled on this date.");
        }

        var added = habit.CheckIns.Add(date);
        if (added)
        {
            SaveToFile();
        }

        return added;
    }

    // Undo check-in for a specific date.
    public bool RemoveCheckIn (HabitDto habit, DateOnly date)
    {
        var removed = habit.CheckIns.Remove(date);
        if (removed)
        {
            SaveToFile();
        }

        return removed;
    }

    // Statistics: calculate habit progress (statistic) through a specific date.
    public HabitStatistics GetHabitStatistics(HabitDto habit, DateOnly throughDate)
    {
        var dates = GetScheduledDates(habit, throughDate).ToList();
        var completed = dates.Count(habit.CheckIns.Contains);
        var percentage = 0.0;
        if (dates.Count > 0)
        {
            percentage = completed * 100.0 / dates.Count;
        }

        // Longest streak => loop through all scheduled dates and count successive checkins.
        var longest = 0;
        var run = 0;
        foreach (var date in dates)
        {
            if (habit.CheckIns.Contains(date))
            {
                run++;
            }
            else
            {
                run = 0;
            }

            longest = Math.Max(longest, run);
        }

        // Current streak => loop backwards from the most recent scheduled date and count successive checkins.
        var current = 0;
        for (var index = dates.Count - 1; index >= 0 && habit.CheckIns.Contains(dates[index]); index--)
        {
            current++;
        }

        return new HabitStatistics(completed, dates.Count, percentage, current, longest);
    }

    // Get list of scheduled dates. 
    private List<DateOnly> GetScheduledDates(HabitDto habit, DateOnly throughDate)
    {
        var dates = new List<DateOnly>();
        for (var date = habit.CreatedOn; date <= throughDate; date = date.AddDays(1))
        {
            if (habit.IsScheduledOn(date))
            {
                dates.Add(date);
            }
        }
        return dates;
    }

    // Save the current list of habits to the file.
    private void SaveToFile()
    {
        repository.SaveFile(habits);
    }

}
