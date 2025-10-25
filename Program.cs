using Microsoft.AspNetCore.Mvc;
using QuizApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register repositories (file-based first; fallback to in-memory if file missing).
builder.Services.AddSingleton<IQuizRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data", "quizzes.json");
    if (File.Exists(dataPath))
    {
        return new FileQuizRepository(dataPath);
    }
    return new InMemoryQuizRepository();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
