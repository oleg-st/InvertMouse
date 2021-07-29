using InvertMouse.Utils;
using System.Runtime.InteropServices;
using System.Threading;

namespace InvertMouse.Inverter
{
    public abstract class InvertMouseBase
    {
        protected Thread Thread;
        protected bool Running;

        public bool IsRunning => Running;
        public CheckState State { get; protected set; }
        public string Error { get; protected set; }
        public double Delay { get; protected set; }
        public bool WhenCursorIsHidden { get; set; }
        public bool XAxis { get; set; }
        public bool YAxis { get; set; }

        protected int XMultiplier => XAxis ? -1 : 1;
        protected int YMultiplier => YAxis ? -1 : 1;

        protected bool IsCursorHidden()
        {
            var cursorInfo = new WinAPI.CURSORINFO { cbSize = Marshal.SizeOf(typeof(WinAPI.CURSORINFO)) };
            WinAPI.GetCursorInfo(ref cursorInfo);
            return (cursorInfo.flags & WinAPI.CURSOR_SHOWING) == 0;
        }

        protected abstract void Worker();
        public abstract string Name { get; }

        public virtual bool Start()
        {
            if (Running)
            {
                return true;
            }

            if (State != CheckState.Ok)
            {
                return false;
            }

            lock (this)
            {
                Thread = new Thread(Worker);
                Running = true;
                Thread.Start();
                return true;
            }
        }

        public virtual void Stop()
        {
            if (!Running)
            {
                return;
            }

            Running = false;
            lock (this)
            {
                Thread.Join();
            }
        }
    }
}
