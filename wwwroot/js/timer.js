window.startCountdown = function (minutes) {
    var totalSeconds = minutes * 60;
    var timerEl = document.getElementById("timer");
    var form = document.querySelector('form[method="post"][action$="Result"]') ||
        document.querySelector('form[action="/Quiz/Result"]');
    function fmt(n) { return n < 10 ? "0" + n : n; }
    function tick() {
        var m = Math.floor(totalSeconds / 60);
        var s = totalSeconds % 60;
        if (timerEl) timerEl.textContent = fmt(m) + ":" + fmt(s);
        if (totalSeconds <= 0) { if (form) form.submit(); return; }
        totalSeconds--; setTimeout(tick, 1000);
    }
    tick();
};

window.startStopwatch = function () {
    var sec = 0, el = document.getElementById("stopwatch");
    function fmt(n) { return n < 10 ? "0" + n : n; }
    setInterval(function () {
        sec++;
        var m = Math.floor(sec / 60), s = sec % 60;
        if (el) el.textContent = fmt(m) + ":" + fmt(s);
    }, 1000);
};
