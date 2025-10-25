using QuizApp.Models;

namespace QuizApp.Services;

public interface IQuizRepository
{
    Quiz GetById(string id);
    IEnumerable<Quiz> GetAll();
}
