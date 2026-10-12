namespace Sep2026.DailyHabitTracker;

public class HabitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly CreatedOn { get; set; }
    public bool IsArchived { get; set; }
    public HashSet<DayOfWeek> ScheduledDays { get; set; } = [];
    public HashSet<DateOnly> CheckIns { get; set; } = [];

    public bool IsScheduledOn(DateOnly date)
    {
        return date >= CreatedOn && ScheduledDays.Contains(date.DayOfWeek);
    }
}

public record HabitStatistics
(
    int Completed, 
    int Scheduled, 
    double CompletionPercentage,
    int CurrentStreak, 
    int LongestStreak
);
