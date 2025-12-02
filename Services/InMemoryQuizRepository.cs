using QuizApp.Models;

namespace QuizApp.Services;

public class InMemoryQuizRepository : IQuizRepository
{
    private readonly List<Quiz> _quizzes;

    public InMemoryQuizRepository()
    {
        _quizzes = new()
        {
            new Quiz
            {
                Id = "mvc",
                Title = "ASP.NET Core MVC Basics",
                Questions = new()
                {
                    new Question { Text = "Which method in a controller returns a View in ASP.NET Core MVC?", Options = new() { "Display()", "Show()", "View()", "Render()" }, CorrectIndex = 2 },
                    new Question { Text = "In MVC, what does 'C' stand for?", Options = new() { "Code", "Controller", "Class", "Content" }, CorrectIndex = 1 },
                    new Question { Text = "Which attribute indicates an HTTP GET action?", Options = new() { "[Get]", "[HttpGet]", "[RouteGet]", "[ActionGet]" }, CorrectIndex = 1 },
                    new Question { Text = "Where do .cshtml files live by convention?", Options = new() { "/Razor", "/Templates", "/Views", "/PagesOnly" }, CorrectIndex = 2 },
                    new Question { Text = "What is the default view engine?", Options = new() { "Razor", "Mustache", "Handlebars", "Liquid" }, CorrectIndex = 0 },
                }
            }
        };
    }

    public InMemoryQuizRepository(List<Quiz> quizzes)
    {
        _quizzes = quizzes ?? new List<Quiz>();
    }

    public IEnumerable<Quiz> GetAll() => _quizzes;

    public Quiz GetById(string id) => _quizzes.FirstOrDefault(q => q.Id == id) ?? _quizzes.First();
}
