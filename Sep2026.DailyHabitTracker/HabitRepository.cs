using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sep2026.DailyHabitTracker;

public sealed class HabitRepository
{
    private string _filePath;

    private JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public HabitRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<HabitDto> LoadFile()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<HabitDto>>(File.ReadAllText(_filePath), _options) ?? [];
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"Please select a valid file path.", exception);
        }
    }

    public void SaveFile(IEnumerable<HabitDto> habits)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryFile = _filePath + ".tmp";
        File.WriteAllText(temporaryFile, JsonSerializer.Serialize(habits, _options));
        File.Move(temporaryFile, _filePath, true);
    }

}
