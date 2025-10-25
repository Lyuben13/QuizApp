// Quiz Timer Management
class QuizTimer {
    constructor() {
        this.countdownInterval = null;
        this.stopwatchInterval = null;
        this.totalSeconds = 0;
        this.elapsedSeconds = 0;
        this.isRunning = false;
        this.warningThreshold = 60; // Show warning when 1 minute left
        this.criticalThreshold = 30; // Show critical warning when 30 seconds left
    }

    startCountdown(minutes) {
        if (this.isRunning) {
            this.stop();
        }

        this.totalSeconds = Math.max(1, minutes) * 60;
        this.elapsedSeconds = 0;
        this.isRunning = true;

        const timerEl = document.getElementById("timer");
        const form = document.querySelector('form[method="post"][action$="Result"]') ||
            document.querySelector('form[action="/Quiz/Result"]');

        if (!timerEl) {
            console.warn("Timer element not found");
            return;
        }

        const updateDisplay = () => {
            const remaining = this.totalSeconds - this.elapsedSeconds;
            const m = Math.floor(remaining / 60);
            const s = remaining % 60;
            
            if (timerEl) {
                timerEl.textContent = this.formatTime(m, s);
                
                // Add visual warnings
                timerEl.className = this.getTimerClass(remaining);
            }

            if (remaining <= 0) {
                this.stop();
                if (form) {
                    // Show warning before auto-submit
                    this.showAutoSubmitWarning();
                    setTimeout(() => form.submit(), 2000);
                }
                return;
            }

            this.elapsedSeconds++;
        };

        // Initial update
        updateDisplay();
        
        // Start interval
        this.countdownInterval = setInterval(updateDisplay, 1000);
    }

    startStopwatch() {
        if (this.stopwatchInterval) {
            clearInterval(this.stopwatchInterval);
        }

        this.elapsedSeconds = 0;
        const stopwatchEl = document.getElementById("stopwatch");

        if (!stopwatchEl) {
            console.warn("Stopwatch element not found");
            return;
        }

        this.stopwatchInterval = setInterval(() => {
            this.elapsedSeconds++;
            const m = Math.floor(this.elapsedSeconds / 60);
            const s = this.elapsedSeconds % 60;
            stopwatchEl.textContent = this.formatTime(m, s);
        }, 1000);
    }

    stop() {
        if (this.countdownInterval) {
            clearInterval(this.countdownInterval);
            this.countdownInterval = null;
        }
        if (this.stopwatchInterval) {
            clearInterval(this.stopwatchInterval);
            this.stopwatchInterval = null;
        }
        this.isRunning = false;
    }

    formatTime(minutes, seconds) {
        return (minutes < 10 ? "0" : "") + minutes + ":" + 
               (seconds < 10 ? "0" : "") + seconds;
    }

    getTimerClass(remainingSeconds) {
        if (remainingSeconds <= this.criticalThreshold) {
            return "badge bg-danger fs-6 text-white";
        } else if (remainingSeconds <= this.warningThreshold) {
            return "badge bg-warning fs-6 text-dark";
        }
        return "badge bg-primary fs-6";
    }

    showAutoSubmitWarning() {
        const warning = document.createElement('div');
        warning.className = 'alert alert-warning alert-dismissible fade show position-fixed';
        warning.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        warning.innerHTML = `
            <i class="bi bi-exclamation-triangle me-2"></i>
            <strong>Time's up!</strong> Submitting your quiz automatically...
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(warning);

        // Auto-remove after 3 seconds
        setTimeout(() => {
            if (warning.parentNode) {
                warning.parentNode.removeChild(warning);
            }
        }, 3000);
    }

    getElapsedTime() {
        return this.elapsedSeconds;
    }

    getRemainingTime() {
        return Math.max(0, this.totalSeconds - this.elapsedSeconds);
    }
}

// Global timer instance
window.quizTimer = new QuizTimer();

// Legacy functions for backward compatibility
window.startCountdown = function(minutes) {
    window.quizTimer.startCountdown(minutes);
};

window.startStopwatch = function() {
    window.quizTimer.startStopwatch();
};

// Auto-save functionality (optional)
window.enableAutoSave = function(intervalSeconds = 30) {
    setInterval(() => {
        if (window.quizTimer && window.quizTimer.isRunning) {
            const form = document.querySelector('form[method="post"][action$="Result"]');
            if (form) {
                // Save form data to localStorage
                const formData = new FormData(form);
                const answers = [];
                for (let [key, value] of formData.entries()) {
                    if (key.startsWith('answers[')) {
                        answers.push(parseInt(value) || -1);
                    }
                }
                localStorage.setItem('quiz_answers', JSON.stringify(answers));
                console.log('Quiz progress auto-saved');
            }
        }
    }, intervalSeconds * 1000);
};

// Restore saved answers (optional)
window.restoreAnswers = function() {
    const saved = localStorage.getItem('quiz_answers');
    if (saved) {
        try {
            const answers = JSON.parse(saved);
            answers.forEach((answer, index) => {
                if (answer >= 0) {
                    const radio = document.querySelector(`input[name="answers[${index}]"][value="${answer}"]`);
                    if (radio) {
                        radio.checked = true;
                    }
                }
            });
            console.log('Quiz answers restored from auto-save');
        } catch (e) {
            console.warn('Failed to restore saved answers:', e);
        }
    }
};

// Clean up on page unload
window.addEventListener('beforeunload', () => {
    if (window.quizTimer) {
        window.quizTimer.stop();
    }
});
