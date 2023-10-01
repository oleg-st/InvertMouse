using InvertMouse.Utils;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace InvertMouse.Inverter
{
    public abstract class InvertMouseBase
    {
        protected Thread thread;

        public bool IsRunning { get; protected set; }
        public CheckState State { get; protected set; }
        public string Error { get; protected set; }
        public double Delay { get; protected set; }
        public bool WhenCursorIsHidden { get; set; }
        public string ActiveTitlePrefix { get; set; }

        public decimal XMultiplier { get; set; }
        public decimal YMultiplier { get; set; }

        public const decimal InvertMultiplier = -1;
        public const decimal IdentityMultiplier = 1;

        bool IsCursorHidden()
        {
            var cursorInfo = new WinAPI.CURSORINFO { cbSize = Marshal.SizeOf(typeof(WinAPI.CURSORINFO)) };
            if (!WinAPI.GetCursorInfo(ref cursorInfo))
            {
                // fails on secure desktop, i.e. in UAC
                return false;
            }
            return (cursorInfo.flags & WinAPI.CURSOR_SHOWING) == 0;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        bool IsActiveWindowPrefixed()
        {
            if (ActiveTitlePrefix == "")
                return true;

            var handle = GetForegroundWindow();
            if (handle == null)
                return false;

            var intLength = GetWindowTextLength(handle) + 1;
            var stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) <= 0)
                return false;

            var title = stringBuilder.ToString();
            return title.StartsWith(ActiveTitlePrefix);
        }

        protected bool IsActive() => (!WhenCursorIsHidden || IsCursorHidden()) && IsActiveWindowPrefixed();

        protected abstract void Worker();
        public abstract string Name { get; }

        public virtual bool Start()
        {
            if (IsRunning)
            {
                return true;
            }

            if (State != CheckState.Ok)
            {
                return false;
            }

            lock (this)
            {
                thread = new Thread(Worker);
                IsRunning = true;
                thread.Start();
                return true;
            }
        }

        public virtual void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            lock (this)
            {
                thread.Join();
            }
        }
    }
}
