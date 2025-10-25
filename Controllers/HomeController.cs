using Microsoft.AspNetCore.Mvc;
using QuizApp.Services;

namespace QuizApp.Controllers;

public class HomeController : Controller
{
    private readonly IQuizRepository _repo;

    public HomeController(IQuizRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public IActionResult Index()
    {
        try
        {
            var quizzes = _repo.GetAll()?.OrderBy(q => q.Title).ToList() ?? new List<QuizApp.Models.Quiz>();
            if (!quizzes.Any())
            {
                // Optional: Log warning or set a view message
                ViewBag.NoQuizzesMessage = "No quizzes available at this time.";
            }
            return View(quizzes);
        }
        catch (Exception ex)
        {
            // Log the exception (e.g., using ILogger)
            Console.WriteLine($"Error loading quizzes: {ex.Message}");
            return View("Error", new { message = "An error occurred while loading quizzes." });
        }
    }
}