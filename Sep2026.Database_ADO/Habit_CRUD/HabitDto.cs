using System;

namespace Sep2026.Database_ADO.Habit_CRUD;

public class HabitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly CreatedOn { get; set; }
    public bool IsArchived { get; set; }
}

