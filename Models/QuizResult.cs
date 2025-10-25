namespace QuizApp.Models;

public class QuizResult
{
    public QuizResult(string quizId, int total, int correct, List<int> submitted, List<int> correctIndices)
    {
        QuizId = quizId ?? throw new ArgumentNullException(nameof(quizId));
        Total = total > 0 ? total : throw new ArgumentException("Total must be positive.", nameof(total));
        Correct = Math.Max(0, Math.Min(correct, total)); // Ensure within valid range
        Submitted = submitted ?? new List<int>();
        CorrectIndices = correctIndices ?? new List<int>();

        if (Submitted.Count != Total || CorrectIndices.Count != Total)
        {
            throw new ArgumentException("Submitted and CorrectIndices lists must match Total count.", nameof(submitted));
        }
    }

    public string QuizId { get; } // Read-only
    public int Total { get; }     // Read-only
    public int Correct { get; }   // Read-only
    public IReadOnlyList<int> Submitted { get; } // Immutable access
    public IReadOnlyList<int> CorrectIndices { get; } // Immutable access

    public bool IsValid => Total > 0 && Correct <= Total;
}