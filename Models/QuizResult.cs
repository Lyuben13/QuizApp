namespace QuizApp.Models;

public class QuizResult
{
    public string QuizId { get; set; } = "default";
    public int Total { get; set; }
    public int Correct { get; set; }
    public List<int> Submitted { get; set; } = new();
    public List<int> CorrectIndices { get; set; } = new();
}
