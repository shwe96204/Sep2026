using Sep2026.DailyHabitTracker;


var dataFile = Path.Combine(Environment.CurrentDirectory, "habits.txt");
var repository = new HabitRepository(dataFile);
var app = new HabitTrackerApp(repository);
app.Start();


