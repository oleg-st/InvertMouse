using System;
using InvertMouse.Utils;
using System.Runtime.InteropServices;
using System.Threading;

namespace InvertMouse.Inverter
{
    public abstract class InvertMouseBase : IDisposable
    {
        protected Thread thread;

        public bool IsRunning { get; protected set; }
        public CheckState State { get; protected set; }
        public string Error { get; protected set; }
        public double Delay { get; protected set; }
        public bool WhenCursorIsHidden { get; set; }

        public decimal XMultiplier { get; set; }
        public decimal YMultiplier { get; set; }

        public const decimal InvertMultiplier = -1;
        public const decimal IdentityMultiplier = 1;

        private readonly CursorTransparencyChecker _cursorTransparencyChecker = new CursorTransparencyChecker();
        protected const int CheckDelayMs = 50;

        protected bool IsCursorHidden()
        {
            var cursorInfo = new WinAPI.CURSORINFO { cbSize = Marshal.SizeOf(typeof(WinAPI.CURSORINFO)) };
            if (!WinAPI.GetCursorInfo(ref cursorInfo))
            {
                // fails on secure desktop, i.e. in UAC
                return false;
            }
            return (cursorInfo.flags & WinAPI.CURSOR_SHOWING) == 0 || _cursorTransparencyChecker.IsCursorFullyTransparent(cursorInfo.hCursor);
        }

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

        public void Dispose()
        {
            _cursorTransparencyChecker.Dispose();
        }
    }
}
