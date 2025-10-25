using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Controllers;

public class QuizController(IQuizRepository repo) : Controller
{
    private readonly IQuizRepository _repo = repo;
    private static readonly Random _rng = new();

    [HttpGet("{id?}")]
    public IActionResult Index(string id = "mvc", bool shuffle=true, int? minutes=null)
    {
        var quiz = _repo.GetById(id);

        // Optionally shuffle questions and answers
        if (shuffle)
        {
            quiz.Questions = quiz.Questions
                .OrderBy(_ => _rng.Next())
                .Select(q => new Question
                {
                    Id = q.Id,
                    Text = q.Text,
                    CorrectIndex = q.CorrectIndex,
                    Options = q.Options.OrderBy(_ => _rng.Next()).ToList()
                }).ToList();

            // After shuffling options, we must re-map CorrectIndex
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var original = _repo.GetById(id).Questions.First(qq => qq.Text == quiz.Questions[i].Text);
                var correctText = original.Options[original.CorrectIndex];
                quiz.Questions[i].CorrectIndex = quiz.Questions[i].Options.FindIndex(x => x == correctText);
            }
        }

        ViewBag.Minutes = minutes ?? 10; // default 10 minutes total
        return View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Result(string quizId, List<int> answers)
    {
        var quiz = _repo.GetById(quizId);
        var total = quiz.Questions.Count;

        // Protect length mismatch
        if (answers.Count != total)
        {
            // Pad with -1 for unanswered
            while (answers.Count < total) answers.Add(-1);
        }

        int correct = 0;
        var correctIndices = new List<int>(total);
        for (int i = 0; i < total; i++)
        {
            var q = quiz.Questions[i];
            correctIndices.Add(q.CorrectIndex);
            if (answers[i] == q.CorrectIndex) correct++;
        }

        var model = new QuizResult
        {
            QuizId = quiz.Id,
            Total = total,
            Correct = correct,
            Submitted = answers,
            CorrectIndices = correctIndices
        };

        ViewBag.QuizTitle = quiz.Title;
        ViewBag.ScoreMessage = correct switch
        {
            var c when c == total => "Perfect! Outstanding!",
            var c when c >= (int)(0.8 * total) => "Great job!",
            var c when c >= (int)(0.5 * total) => "Good try — keep practicing!",
            _ => "Try again!"
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Retake(string quizId)
    {
        return RedirectToAction(nameof(Index), new { id = quizId });
    }
}
