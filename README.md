# 🧭 QuizApp — ASP.NET Core MVC

### 🧭 QuizApp — ASP.NET Core MVC

Учебно приложение за викторина, разработено с **ASP.NET Core MVC**.
Проектът демонстрира основите на MVC архитектурата – модели, изгледи, контролери, маршрутизация и работа с формуляри без използване на база данни.

> A learning project — a simple quiz application built with **ASP.NET Core MVC**.
> It demonstrates the fundamentals of the MVC architecture — models, views, controllers, routing, and form handling without a database.

---

## ⚙️ Изисквания

### ⚙️ Requirements

* .NET SDK 8.0 или по-нова версия
* Visual Studio 2022 или Visual Studio Code с поддръжка на C#
* (По желание) JSON редактор за добавяне на въпроси

> - .NET SDK 8.0 or newer
> - Visual Studio 2022 or Visual Studio Code with C# support
> - (Optional) JSON editor for editing quiz data

---

## 🚀 Стартиране

### 🚀 How to Run

Отвори терминал в основната папка на проекта и изпълни:

```bash
dotnet restore
dotnet run
```

След това отвори показания адрес (например: [http://localhost:5000](http://localhost:5000)).

> Open a terminal in the project folder and run the above commands.
> Then open the URL shown in the console (usually [http://localhost:5000](http://localhost:5000)).

---

## 📋 Основни функционалности

### 📋 Main Features

* 🏠 Начална страница със списък от налични тестове
* ❓ Въпроси с по четири възможни отговора (A–D)
* ✅ Изчисляване на резултатите и показване на верните отговори
* 🔁 Възможност за повторно решаване (**Retake Quiz**)
* 🔀 Разбъркване на въпросите и отговорите при всяко зареждане
* ⏱️ Таймер с обратно броене (по подразбиране 10 минути)
* 🗂️ Зареждане на тестове от JSON файл (`Data/quizzes.json`)

> - 🏠 Home page with a list of available quizzes
> - ❓ Multiple-choice questions (A–D)
> - ✅ Score calculation and feedback on correct answers
> - 🔁 **Retake Quiz** button for retrying
> - 🔀 Random shuffle of questions and options on each load
> - ⏱️ Countdown timer (default 10 minutes)
> - 🗂️ Data loaded from a JSON file (`Data/quizzes.json`)

---

## 🧱 Структура на проекта

### 🧱 Project Structure

```
Controllers/
  HomeController.cs
  QuizController.cs
Models/
  Question.cs
  Quiz.cs
  QuizResult.cs
Services/
  IQuizRepository.cs
  FileQuizRepository.cs
  InMemoryQuizRepository.cs
Views/
  Home/Index.cshtml
  Quiz/Index.cshtml
  Quiz/Result.cshtml
Data/
  quizzes.json
wwwroot/
  css/site.css
  js/timer.js
Program.cs
QuizApp.csproj
```

> The project follows a clean MVC structure with separate folders for controllers, models, views, services, and static resources.

---

## 🧩 Данни

### 🧩 Quiz Data

Въпросите се съхраняват във файл **`Data/quizzes.json`**, което позволява лесно добавяне на нови тестове без промени в кода.

> Quiz questions are stored in a JSON file (`Data/quizzes.json`), making it easy to add or modify quizzes without touching the source code.

Примерен запис:

```json
{
  "Text": "Which method in a controller returns a View?",
  "Options": ["Display()", "Show()", "View()", "Render()"],
  "CorrectIndex": 2
}
```

---

## 🧪 Демонстрация

### 🧪 How to Test

1. Стартирай проекта (`dotnet run`)
2. Избери тест от началната страница
3. Отговори на въпросите и натисни **Submit**
4. Прегледай резултата и използвай **Retake Quiz**, за да повториш

> 1) Run the project (`dotnet run`)
> 2) Select a quiz from the home page
> 3) Answer the questions and click **Submit**
> 4) View your score and use **Retake Quiz** to try again

---

## 🛠️ Допълнителни настройки

### 🛠️ Configuration Tips

* ⏱️ Промени времето за решаване чрез `/Quiz/Index?id=mvc&minutes=15`
* 🔄 Спри разбъркването на въпросите с `/Quiz/Index?id=mvc&shuffle=false`
* 🖌️ Персонализирай стила чрез `wwwroot/css/site.css`

> - ⏱️ Change timer duration via `/Quiz/Index?id=mvc&minutes=15`
> - 🔄 Disable shuffling with `/Quiz/Index?id=mvc&shuffle=false`
> - 🖌️ Customize layout and colors in `wwwroot/css/site.css`

---

## 📄 Бележки

### 📄 Notes

Проектът е създаден за учебни цели и представя основите на **Model–View–Controller** архитектурата в .NET.
Може да бъде разширен с база данни, система за потребители или повече категории тестове.

> This project was created for educational purposes to demonstrate the fundamentals of the **Model–View–Controller** pattern in .NET.
> It can be extended with a database, user authentication, or quiz categories.


