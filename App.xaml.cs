using System.Windows;

namespace WinAiAgent
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                bool isDark = IsSystemDarkTheme();
                if (isDark)
                {
                    Resources["TextForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240,240,240));
                    Resources["SearchIconBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200,200,200));
                    Resources["SearchBorderBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));
                    Resources["SearchBackgroundTop"] = System.Windows.Media.Color.FromArgb(0x66, 0x44, 0x44, 0x44);
                    Resources["SearchBackgroundBottom"] = System.Windows.Media.Color.FromArgb(0x66, 0x33, 0x33, 0x33);
                    Resources["ToastBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xF2, 0x22, 0x22, 0x22));
                    Resources["ToastForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                }
                else
                {
                    Resources["TextForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34,34,34));
                    Resources["SearchIconBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(102,102,102));
                    Resources["SearchBorderBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x1F, 0x00, 0x00, 0x00));
                    Resources["SearchBackgroundTop"] = System.Windows.Media.Color.FromArgb(0xB3, 0xFF, 0xFF, 0xFF);
                    Resources["SearchBackgroundBottom"] = System.Windows.Media.Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF);
                    Resources["ToastBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xF2, 0xFF, 0xFF, 0xFF));
                    Resources["ToastForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34,34,34));
                }
            }
            catch { }
        }

        private static bool IsSystemDarkTheme()
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
                if (key != null)
                {
                    var appsUseLightTheme = key.GetValue("AppsUseLightTheme");
                    if (appsUseLightTheme is int i)
                    {
                        return i == 0; // 0 = dark, 1 = light
                    }
                }
            }
            catch { }
            return false;
        }
    }
}



