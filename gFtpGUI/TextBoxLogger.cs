using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace gFtpGUI
{
    public class TextBoxLogger : ILogger
    {
        private readonly TextBox _textBox;

        public TextBoxLogger(TextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = "";
            if (formatter != null)
            {
                message += formatter(state, exception);
            }

            _textBox.Invoke(new Action(() => _textBox.AppendText($"[{DateTime.Now:yyy-MM-ff HH:mm:ss}][{logLevel}][{eventId.Id}]{message}{Environment.NewLine}")));
        }
    }
}
