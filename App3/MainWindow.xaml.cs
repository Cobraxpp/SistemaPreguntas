using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;

namespace App3
{
    public partial class MainWindow : Window
    {
        private int timeLeft;
        private int currentQuestionIndex;
        private List<(string Question, string Answer)> questions;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuestions();
            StartNewGame();
        }

        private void InitializeQuestions()
        {
            questions = new List<(string Question, string Answer)>
            {
              ("¿Quién escribió 'Don Quijote de la Mancha'?", "Miguel de Cervantes"),
("¿Cuál es el río más largo del mundo?", "El Amazonas"),
("¿En qué año llegó el hombre a la Luna?", "En 1969"),
("¿Cuál es el elemento químico con símbolo 'O'?", "El oxígeno"),
("¿Cuántos huesos tiene el cuerpo humano en la edad adulta?", "206"),
("¿Quién pintó la Mona Lisa?", "Leonardo da Vinci"),
("¿Cuál es el país con más habitantes del mundo?", "China"),
("¿Qué significa 'E=mc²'?", "La equivalencia entre la energía y la masa según la teoría de la relatividad de Einstein"),
("¿Cuál es el idioma más hablado del mundo?", "El inglés"),
("¿Cuánto dura un año en la Tierra?", "365 días")  
                // Añade más preguntas según sea necesario
            };
        }

        private void StartNewGame()
        {
            // Set the initial time limit (e.g., 10 seconds)
            timeLeft = 60;
            TimerTextBlock.Text = $"Time Left: {timeLeft}s";

            // Start with the first question
            currentQuestionIndex = 0;
            DisplayCurrentQuestion();

            // Clear the previous answer feedback
            FeedbackTextBlock.Text = "";
            FeedbackTextBlock.Background = null;

            // Initialize the progress bar
            QuestionProgressBar.Minimum = 0;
            QuestionProgressBar.Maximum = questions.Count;
            QuestionProgressBar.Value = 0;

            // Start the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void DisplayCurrentQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];
                QuestionTextBlock.Text = question.Question;
                QuestionProgressBar.Value = currentQuestionIndex;
            }
            else
            {
                // End of the game
                QuestionTextBlock.Text = "¡Has respondido todas las preguntas!";
                AnswerTextBox.IsEnabled = false;
                SubmitButton.IsEnabled = false;
                timer.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                // Update the timer
                timeLeft--;
                TimerTextBlock.Text = $"Time Left: {timeLeft}s";
            }
            else
            {
                // Time is up
                timer.Stop();
                FeedbackTextBlock.Text = "¡Se acabó el tiempo!";
                FeedbackTextBlock.Background = System.Windows.Media.Brushes.Red;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
        }

        private void AnswerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckAnswer();
            }
        }

        private void CheckAnswer()
        {
            // Normalize and remove diacritics (accents) from the user's answer and the correct answer
            string userAnswer = RemoveDiacritics(AnswerTextBox.Text.Trim());
            string correctAnswer = RemoveDiacritics(questions[currentQuestionIndex].Answer);

            // Check the user's answer
            if (userAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
            {
                FeedbackTextBlock.Text = "¡Correcto!";
                FeedbackTextBlock.Background = System.Windows.Media.Brushes.Green;

                // Add 10 seconds to the timer
                timeLeft += 10;
                TimerTextBlock.Text = $"Time Left: {timeLeft}s";

                // Move to the next question
                currentQuestionIndex++;
                DisplayCurrentQuestion();
            }
            else
            {
                FeedbackTextBlock.Text = "¡Incorrecto!";
                FeedbackTextBlock.Background = System.Windows.Media.Brushes.Red;
            }

            // Clear the answer text box
            AnswerTextBox.Text = "";
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}