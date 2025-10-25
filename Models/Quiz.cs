namespace QuizApp.Models;

public class Quiz
{
    public string Id { get; set; } = "default";
    public string Title { get; set; } = "MVC Basics Quiz";
    public List<Question> Questions { get; set; } = new();
}
