using System;
using System.Runtime.InteropServices;
using System.Threading;
using InvertMouse.Utils;

namespace InvertMouse
{
    public class InvertMouse
    {
        public enum CheckState
        {
            Ok,
            LibraryNotLoaded,
            DriverNotInstalled,
        }

        private Thread _thread;
        private bool _running;
        private CheckState _state;

        public InvertMouse()
        {
            Check();
        }

        public bool IsRunning => _running;
        public CheckState State => _state;

        public bool WhenCursorIsHidden { get; set; }
        public bool XAxis { get; set; }
        public bool YAxis { get; set; }

        private void Check()
        {
            if (Interception.LoadInterception() == IntPtr.Zero)
            {
                _state = CheckState.LibraryNotLoaded;
                return;
            }

            var context = Interception.interception_create_context();
            if (context == IntPtr.Zero)
            {
                _state = CheckState.DriverNotInstalled;
                return;
            }

            Interception.interception_destroy_context(context);
            _state = CheckState.Ok;
        }

        private bool IsCursorHidden()
        {
            var cursorInfo = new WinAPI.CURSORINFO {cbSize = Marshal.SizeOf(typeof(WinAPI.CURSORINFO))};
            WinAPI.GetCursorInfo(ref cursorInfo);
            return (cursorInfo.flags & WinAPI.CURSOR_SHOWING) == 0;
        }

        private void Worker()
        {
            WinAPI.RaiseProcessPriority();

            var context = Interception.interception_create_context();

            Interception.interception_set_filter(context, Interception.interception_is_mouse,
                (ushort) Interception.InterceptionFilterMouseState.INTERCEPTION_FILTER_MOUSE_MOVE);

            var stroke = new Interception.InterceptionMouseStroke();
            while (_running)
            {
                var device = Interception.interception_wait_with_timeout(context, 500);
                if (
                    Interception.interception_is_mouse(device) != 0 &&
                    Interception.interception_receive(context, device, ref stroke, 1) > 0
                )
                {
                    if (
                        (stroke.flags & (ushort) Interception.InterceptionMouseFlag.INTERCEPTION_MOUSE_MOVE_ABSOLUTE) == 0
                        && (!WhenCursorIsHidden || IsCursorHidden())
                    )
                    {
                        if (XAxis)
                            stroke.x *= -1;

                        if (YAxis)
                            stroke.y *= -1;
                    }

                    Interception.interception_send(context, device, ref stroke, 1);
                }
            }

            Interception.interception_destroy_context(context);
        }

        public bool Start()
        {
            if (_running)
            {
                return true;
            }

            if (_state != CheckState.Ok)
            {
                return false;
            }

            lock (this)
            {
                _thread = new Thread(Worker);
                _running = true;
                _thread.Start();
                return true;
            }
        }

        public void Stop()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            lock (this)
            {
                _thread.Join();
            }
        }
    }
}
