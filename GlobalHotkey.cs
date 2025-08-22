using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WinAiAgent
{
    public sealed class GlobalHotkey : IDisposable
    {
        private readonly int _id;
        private readonly IntPtr _handle;
        private readonly HwndSource _source;

        private const int WM_HOTKEY = 0x0312;
        private bool _registered;

        [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum Modifiers : uint
        {
            None = 0x0000,
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public event EventHandler? Pressed;

        public GlobalHotkey(IntPtr handle, HwndSource source, Modifiers modifiers, uint virtualKey, int id)
        {
            _handle = handle;
            _source = source;
            _id = id;
            _source.AddHook(WndProc);
            _registered = RegisterHotKey(_handle, _id, (uint)modifiers, virtualKey);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
            {
                Pressed?.Invoke(this, EventArgs.Empty);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            try
            {
                if (_registered)
                {
                    UnregisterHotKey(_handle, _id);
                    _registered = false;
                }
            }
            catch
            {
                // ignore
            }

            _source.RemoveHook(WndProc);
        }
    }
}



