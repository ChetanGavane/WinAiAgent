using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace WinAiAgent
{
    public partial class MainWindow : Window
    {
        private GlobalHotkey? _hotkey;
        private GlobalHotkey? _hotkeyAlt;
        private GlobalHotkey? _hotkeyCtrlShift;
        private SpeechRecognition? _speech;
        private readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public MainWindow()
        {
            InitializeComponent();
            Deactivated += (_, __) => HideOverlay();
            Closed += (_, __) =>
            {
                try { _speech?.Dispose(); } catch { }
                try { _hotkey?.Dispose(); } catch { }
                try { _hotkeyAlt?.Dispose(); } catch { }
                try { _hotkeyCtrlShift?.Dispose(); } catch { }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var workArea = SystemParameters.WorkArea;
            Width = 500; // outer window width, larger than bar
            Height = 120;
            Left = workArea.Left + (workArea.Width - Width) / 2.0;
            Top = workArea.Top + 50;

            var interop = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(interop.Handle)!;
            _hotkey = new GlobalHotkey(interop.Handle, source, GlobalHotkey.Modifiers.Control, 0x20 /*Space*/, 1);
            _hotkey.Pressed += (_, __) => ToggleOverlay();
            _hotkeyCtrlShift = new GlobalHotkey(interop.Handle, source, GlobalHotkey.Modifiers.Control | GlobalHotkey.Modifiers.Shift, 0x20 /*Space*/, 2);
            _hotkeyCtrlShift.Pressed += (_, __) => ToggleOverlay();
            _hotkeyAlt = new GlobalHotkey(interop.Handle, source, GlobalHotkey.Modifiers.Alt, 0x20 /*Space*/, 3);
            _hotkeyAlt.Pressed += (_, __) => ToggleOverlay();

            _speech = new SpeechRecognition();
            _speech.TextRecognized += (_, text) => Dispatcher.Invoke(() => InputBox.Text = text);
            _speech.ErrorOccurred += (_, message) => ShowToast($"Speech error: {message}");

            // Show immediately on startup (text input active). Voice starts only when mic is clicked.
            ShowOverlay();
        }

        private void ToggleOverlay()
        {
            if (Opacity > 0.01)
            {
                HideOverlay();
            }
            else
            {
                ShowOverlay();
            }
        }

        private void ShowOverlay()
        {
            Show();
            Activate();
            InputBox.Focus();
            if (_speech != null)
            {
                try { _speech.Start(); } catch { }
            }
            FadeTo(1.0);
            if (string.IsNullOrWhiteSpace(InputBox.Text))
            {
                InputBox.Text = string.Empty;
                InputBox.PlaceholderText("");
            }
        }

        private void HideOverlay(bool immediate = false)
        {
            if (_speech != null)
            {
                try { _speech.Stop(); } catch { }
            }

            if (immediate)
            {
                Opacity = 0;
                Hide();
                return;
            }

            FadeTo(0.0, onCompleted: () => Hide());
        }

        private void FadeTo(double target, Action? onCompleted = null)
        {
            var animation = new DoubleAnimation
            {
                To = target,
                Duration = TimeSpan.FromMilliseconds(300),
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.2
            };
            if (onCompleted != null)
            {
                animation.Completed += (_, __) => onCompleted();
            }
            BeginAnimation(OpacityProperty, animation);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendCommandAsync();
        }

        private void MicButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _speech?.Start();
                ShowToast("Listening...");
            }
            catch (Exception ex)
            {
                ShowToast($"Mic error: {ex.Message}");
            }
        }

        private async Task SendCommandAsync()
        {
            var text = InputBox.Text?.Trim();
            if (string.IsNullOrEmpty(text))
            {
                ShowToast("Type something to send.");
                return;
            }

            try
            {
                StartActivityRing();
                var payload = new { command = text };
                var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/execute-command", payload);
                if (!response.IsSuccessStatusCode)
                {
                    ShowToast($"HTTP {((int)response.StatusCode)}");
                    return;
                }
                using var stream = await response.Content.ReadAsStreamAsync();
                var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.TryGetProperty("status", out var status) && status.GetString() == "success")
                {
                    var result = doc.RootElement.TryGetProperty("result", out var r) ? r.GetString() : "OK";
                    ShowToast(result ?? "OK");
                }
                else
                {
                    var msg = doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() : "Error";
                    if (doc.RootElement.TryGetProperty("echo", out var echo))
                    {
                        msg = $"{msg} | echo: {echo.GetString()}";
                    }
                    ShowToast(msg ?? "Error");
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Backend error: {ex.Message}");
            }
            finally
            {
                StopActivityRing();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideOverlay();
                e.Handled = true;
            }
            else if (e.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Application.Current.Shutdown();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                _ = SendCommandAsync();
                e.Handled = true;
            }
        }

        private void InputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // placeholder handled via extension
        }

        private void ShowToast(string message)
        {
            Debug.WriteLine($"[WindowsSearchApp] {message}");
            ToastText.Text = message;
            var fadeIn = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            var fadeOut = new DoubleAnimation
            {
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(2),
                Duration = TimeSpan.FromMilliseconds(300)
            };
            var storyboard = new Storyboard();
            Storyboard.SetTarget(fadeIn, Toast);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(fadeOut, Toast);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(fadeOut);
            storyboard.Begin();
        }

        private void StartActivityRing()
        {
            ActivityRing.Opacity = 1.0;
            var rotateGradient = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromMilliseconds(1200),
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Rotate the gradient of the stroke, not the shape (keeps corners clean)
            if (ActivityRing.Stroke is System.Windows.Media.LinearGradientBrush brush && brush.RelativeTransform is System.Windows.Media.RotateTransform rt)
            {
                rt.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, rotateGradient);
            }
        }

        private void StopActivityRing()
        {
            ActivityRing.Opacity = 0.0;
            if (ActivityRing.Stroke is System.Windows.Media.LinearGradientBrush brush && brush.RelativeTransform is System.Windows.Media.RotateTransform rt)
            {
                rt.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, null);
            }
        }
    }

    public static class TextBoxExtensions
    {
        public static void PlaceholderText(this System.Windows.Controls.TextBox textBox, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
                textBox.GotFocus += RemoveOnFocus;
                textBox.LostFocus += RestoreOnBlur;

                void RemoveOnFocus(object? s, RoutedEventArgs e)
                {
                    if (textBox.Text == placeholder)
                    {
                        textBox.Text = string.Empty;
                        textBox.Foreground = System.Windows.Media.Brushes.Black;
                    }
                }

                void RestoreOnBlur(object? s, RoutedEventArgs e)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = placeholder;
                        textBox.Foreground = System.Windows.Media.Brushes.Gray;
                    }
                }
            }
        }
    }
}


