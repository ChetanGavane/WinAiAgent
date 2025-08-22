using System;
using System.Collections.Generic;
using Windows.Media.SpeechRecognition;
using Windows.Foundation;
using System.Threading.Tasks;

namespace WinAiAgent
{
    public sealed class SpeechRecognition : IDisposable
    {
        private SpeechRecognizer? _recognizer;

        public event EventHandler<string>? TextRecognized;
        public event EventHandler<string>? ErrorOccurred;

        public SpeechRecognition() { }

        public async void Start()
        {
            try
            {
                if (_recognizer == null)
                {
                    _recognizer = new SpeechRecognizer();
                    await _recognizer.CompileConstraintsAsync();
                    _recognizer.ContinuousRecognitionSession.ResultGenerated += OnResultGenerated;
                }
                await _recognizer.ContinuousRecognitionSession.StartAsync();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
            }
        }

        private void OnResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            string text = args.Result.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                TextRecognized?.Invoke(this, text);
            }
        }

        public async void Stop()
        {
            try
            {
                if (_recognizer != null)
                {
                    await _recognizer.ContinuousRecognitionSession.StopAsync();
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            try { _recognizer?.Dispose(); } catch { }
        }
    }
}


