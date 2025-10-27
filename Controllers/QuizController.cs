using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Controllers;

public class QuizController : Controller
{
    private readonly IQuizRepository _repo;
    private readonly ILogger<QuizController> _logger;
    private static readonly Random _rng = new();

    public QuizController(IQuizRepository repo, ILogger<QuizController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index(string id = "mvc", bool shuffle = false, int? minutes = null)
    {
        try
    {
        var quiz = _repo.GetById(id);
        if (quiz == null)
            {
                var baseQuiz = _repo.GetById(id);
                if (baseQuiz == null) return View("NotFound");
                _logger.LogWarning("Quiz with id '{QuizId}' not found", id);
            return NotFound($"Quiz with id '{id}' not found.");
            }

            // Validate quiz has questions
            if (!quiz.Questions.Any())
            {
                _logger.LogWarning("Quiz '{QuizId}' has no questions", id);
                return BadRequest("This quiz has no questions available.");
            }

            // Create a copy to avoid modifying the original
            var quizCopy = new Quiz
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = quiz.Questions.Select(q => new Question
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = new List<string>(q.Options),
                    CorrectIndex = q.CorrectIndex
                }).ToList()
            };

            // Shuffle if requested
            if (shuffle)
            {
                ShuffleQuiz(quizCopy);
            }

            ViewBag.Minutes = Math.Max(1, minutes ?? 10); // Ensure minimum 1 minute
            ViewBag.IsShuffled = shuffle;
            ViewBag.QuizId = id;

            return View(quizCopy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading quiz with id '{QuizId}'", id);
            return StatusCode(500, "An error occurred while loading the quiz.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Result(string quizId, List<int> answers)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(quizId))
            {
                _logger.LogWarning("Empty quizId provided in Result action");
                return BadRequest("Quiz ID is required.");
            }

            // Get the original quiz data (not shuffled)
            var originalQuiz = _repo.GetById(quizId);
            if (originalQuiz == null)
            {
                _logger.LogWarning("Quiz with id '{QuizId}' not found in Result action", quizId);
                //return NotFound("Quiz not found.");
                return View("NotFound");
            }

            // Normalize answers to match question count
            var total = originalQuiz.Questions.Count;
            var normalizedAnswers = NormalizeAnswers(answers, total);

            // Calculate results using the original quiz data
            var result = CalculateQuizResult(originalQuiz, normalizedAnswers);

            // Set view data
            ViewBag.QuizTitle = originalQuiz.Title;
            ViewBag.ScoreMessage = GetScoreMessage(result.Correct, result.Total);
            ViewBag.Percentage = CalculatePercentage(result.Correct, result.Total);

            _logger.LogInformation("Quiz '{QuizId}' completed: {Correct}/{Total} correct", 
                quizId, result.Correct, result.Total);

            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing quiz result for quiz '{QuizId}'", quizId);
            return StatusCode(500, "An error occurred while processing your results.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Retake(string quizId)
    {
        if (string.IsNullOrWhiteSpace(quizId))
        {
            _logger.LogWarning("Empty quizId provided in Retake action");
            return RedirectToAction("Index", "Home");
        }

        _logger.LogInformation("User retaking quiz '{QuizId}'", quizId);
        return RedirectToAction(nameof(Index), new { id = quizId });
    }

    #region Private Helper Methods

    private void ShuffleQuiz(Quiz quiz)
    {
        // Create a deep copy of questions to avoid modifying the original
        var questionsToShuffle = quiz.Questions.Select(q => new Question
        {
            Id = q.Id,
            Text = q.Text,
            Options = new List<string>(q.Options),
            CorrectIndex = q.CorrectIndex
        }).ToList();
        
        // Shuffle questions using Fisher-Yates algorithm
        for (int i = questionsToShuffle.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (questionsToShuffle[i], questionsToShuffle[j]) = (questionsToShuffle[j], questionsToShuffle[i]);
        }
        
        for (int i = 0; i < questionsToShuffle.Count; i++)
        {
            var question = questionsToShuffle[i];
            var originalOptions = question.Options.ToList();
            var correctAnswer = originalOptions[question.CorrectIndex];
            
            // Shuffle options using Fisher-Yates algorithm
            for (int k = originalOptions.Count - 1; k > 0; k--)
            {
                int l = _rng.Next(k + 1);
                (originalOptions[k], originalOptions[l]) = (originalOptions[l], originalOptions[k]);
            }
            
            question.Options = originalOptions;
            
            // Update correct index
            question.CorrectIndex = originalOptions.IndexOf(correctAnswer);
            
            if (question.CorrectIndex == -1)
            {
                _logger.LogError("Correct answer not found after shuffling for question {QuestionId}", question.Id);
                throw new InvalidOperationException($"Correct answer not found after shuffling for question {question.Id}");
            }
        }
        
        quiz.Questions = questionsToShuffle;
    }

    private List<int> NormalizeAnswers(List<int> answers, int totalQuestions)
    {
        if (answers == null)
            return Enumerable.Repeat(-1, totalQuestions).ToList();

        var normalized = new List<int>(answers);
        
        // Pad with -1 if not enough answers
        while (normalized.Count < totalQuestions)
        {
            normalized.Add(-1);
        }
        
        // Truncate if too many answers
        if (normalized.Count > totalQuestions)
        {
            normalized = normalized.Take(totalQuestions).ToList();
        }
        
        return normalized;
    }

    private QuizResult CalculateQuizResult(Quiz quiz, List<int> answers)
    {
        var total = quiz.Questions.Count;
        var correct = 0;
        var correctIndices = new List<int>(total);

        for (int i = 0; i < total; i++)
        {
            var correctIndex = quiz.Questions[i].CorrectIndex;
            correctIndices.Add(correctIndex);
            
            
            if (answers[i] == correctIndex)
            {
                correct++;
            }
        }

        return new QuizResult(quiz.Id, total, correct, answers, correctIndices);
    }

    private string GetScoreMessage(int correct, int total)
    {
        var percentage = CalculatePercentage(correct, total);
        
        return percentage switch
        {
            100 => "Perfect! Outstanding performance! 🎉",
            >= 90 => "Excellent! Great job! 👏",
            >= 80 => "Very good! Well done! 👍",
            >= 70 => "Good work! Keep it up! 💪",
            >= 60 => "Not bad! Try to improve! 📈",
            >= 50 => "You're getting there! Practice more! 📚",
            _ => "Don't give up! Review the material and try again! 🔄"
        };
    }

    private int CalculatePercentage(int correct, int total)
    {
        if (total == 0) return 0;
        return (int)Math.Round(100.0 * correct / total);
    }

    #endregion
}