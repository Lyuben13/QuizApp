using System.Text.Json;
using QuizApp.Models;

namespace QuizApp.Services;

public class FileQuizRepository : IQuizRepository
{
    private readonly string _filePath;
    private readonly Dictionary<string, Quiz> _quizzes;

    public FileQuizRepository(string filePath)
    {
        _filePath = filePath;
        using var stream = File.OpenRead(_filePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var quizzes = JsonSerializer.Deserialize<List<Quiz>>(stream, options) ?? new List<Quiz>();
        // Ensure IDs are present
        foreach (var q in quizzes)
        {
            if (string.IsNullOrWhiteSpace(q.Id)) q.Id = Guid.NewGuid().ToString();
            foreach (var qu in q.Questions)
            {
                if (string.IsNullOrWhiteSpace(qu.Id)) qu.Id = Guid.NewGuid().ToString();
            }
        }
        _quizzes = quizzes.ToDictionary(x => x.Id, x => x);
    }

    public IEnumerable<Quiz> GetAll() => _quizzes.Values;

    public Quiz GetById(string id)
    {
        if (_quizzes.TryGetValue(id, out var quiz))
            return quiz;
        throw new KeyNotFoundException($"Quiz '{id}' not found in {_filePath}");
    }
}
