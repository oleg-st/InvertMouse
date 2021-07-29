using InvertMouse.Utils;
using System;

namespace InvertMouse.Inverter
{
    /*
     * Using Interception driver
     *
     * https://github.com/oblitum/Interception
     */
    public class InvertMouseInterception : InvertMouseBase
    {
        public InvertMouseInterception()
        {
            Check();
        }

        private void Check()
        {
            if (InterceptionLib.LoadInterception() == IntPtr.Zero)
            {
                State = CheckState.LibraryNotLoaded;
                return;
            }

            var context = InterceptionLib.interception_create_context();
            if (context == IntPtr.Zero)
            {
                State = CheckState.DriverNotInstalled;
                return;
            }

            InterceptionLib.interception_destroy_context(context);
            State = CheckState.Ok;
        }

        protected override void Worker()
        {
            WinAPI.RaiseProcessPriority();

            var context = InterceptionLib.interception_create_context();

            InterceptionLib.interception_set_filter(context, InterceptionLib.interception_is_mouse,
                (ushort)InterceptionLib.InterceptionFilterMouseState.INTERCEPTION_FILTER_MOUSE_MOVE);

            var stroke = new InterceptionLib.InterceptionMouseStroke();
            while (Running)
            {
                var device = InterceptionLib.interception_wait_with_timeout(context, 500);
                if (
                    InterceptionLib.interception_is_mouse(device) != 0 &&
                    InterceptionLib.interception_receive(context, device, ref stroke, 1) > 0
                )
                {
                    if (
                        (stroke.flags &
                         (ushort)InterceptionLib.InterceptionMouseFlag.INTERCEPTION_MOUSE_MOVE_ABSOLUTE) == 0
                        && (!WhenCursorIsHidden || IsCursorHidden())
                    )
                    {
                        stroke.x *= XMultiplier;
                        stroke.y *= YMultiplier;
                    }

                    InterceptionLib.interception_send(context, device, ref stroke, 1);
                }
            }

            InterceptionLib.interception_destroy_context(context);
        }

        public override string Name => "Interception";
    }
}
