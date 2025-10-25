using Microsoft.AspNetCore.Mvc;
using QuizApp.Services;

namespace QuizApp.Controllers;

public class HomeController(IQuizRepository repo) : Controller
{
    private readonly IQuizRepository _repo = repo;

    public IActionResult Index()
    {
        var quizzes = _repo.GetAll();
        return View(quizzes);
    }
}
