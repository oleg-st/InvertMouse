using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using InterceptionContext = System.IntPtr;
using InterceptionDevice = System.Int32;
using InterceptionFilter = System.UInt16;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace InvertMouse.Utils
{
    /*
     * Interception library wrapper
     *
     * https://github.com/oblitum/Interception
     */
    static class InterceptionLib
    {
        private const string InterceptionLibrary = "interception";

        public static string GetInterceptionPath()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName == null)
            {
                return null;
            }

            if (Environment.Is64BitProcess)
            {
                return Path.Combine(directoryName, "lib", "x64", "interception.dll");
            }

            return Path.Combine(directoryName, "lib", "x86", "interception.dll");
        }

        public static IntPtr LoadInterception()
        {
            var path = GetInterceptionPath();
            return path != null ? WinAPI.LoadLibraryEx(path, IntPtr.Zero, 0) : IntPtr.Zero;
        }

        [DllImport(InterceptionLibrary)]
        public static extern InterceptionContext interception_create_context();

        [DllImport(InterceptionLibrary)]
        public static extern void interception_destroy_context(InterceptionContext context);

        [DllImport(InterceptionLibrary)]
        public static extern InterceptionDevice interception_wait(InterceptionContext context);

        [DllImport(InterceptionLibrary)]
        public static extern InterceptionDevice interception_wait_with_timeout(InterceptionContext context,
            ulong milliseconds);

        [DllImport(InterceptionLibrary)]
        public static extern int interception_receive(InterceptionContext context, InterceptionDevice device,
            ref InterceptionMouseStroke stroke, uint nstroke);

        [DllImport(InterceptionLibrary)]
        public static extern int interception_send(InterceptionContext context, InterceptionDevice device,
            ref InterceptionMouseStroke stroke, uint nstroke);

        public delegate int InterceptionPredicate(InterceptionDevice device);

        [DllImport(InterceptionLibrary)]
        public static extern void interception_set_filter(InterceptionContext context, InterceptionPredicate predicate,
            InterceptionFilter filter);

        [DllImport(InterceptionLibrary)]
        public static extern int interception_is_keyboard(InterceptionDevice device);

        [DllImport(InterceptionLibrary)]
        public static extern int interception_is_mouse(InterceptionDevice device);

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct InterceptionMouseStroke
        {
            public ushort state;
            public ushort flags;
            public short rolling;
            public int x;
            public int y;
            public uint information;
        }

        public struct InterceptionKeyStroke
        {
            public ushort code;
            public ushort state;
            public uint information;
        }

        #endregion

        #region Enums

        public enum InterceptionMouseState
        {
            INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN = 0x001,
            INTERCEPTION_MOUSE_LEFT_BUTTON_UP = 0x002,
            INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN = 0x004,
            INTERCEPTION_MOUSE_RIGHT_BUTTON_UP = 0x008,
            INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN = 0x010,
            INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP = 0x020,

            INTERCEPTION_MOUSE_BUTTON_1_DOWN = INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_1_UP = INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
            INTERCEPTION_MOUSE_BUTTON_2_DOWN = INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_2_UP = INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
            INTERCEPTION_MOUSE_BUTTON_3_DOWN = INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_3_UP = INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,

            INTERCEPTION_MOUSE_BUTTON_4_DOWN = 0x040,
            INTERCEPTION_MOUSE_BUTTON_4_UP = 0x080,
            INTERCEPTION_MOUSE_BUTTON_5_DOWN = 0x100,
            INTERCEPTION_MOUSE_BUTTON_5_UP = 0x200,

            INTERCEPTION_MOUSE_WHEEL = 0x400,
            INTERCEPTION_MOUSE_HWHEEL = 0x800
        };

        public enum InterceptionFilterMouseState
        {
            INTERCEPTION_FILTER_MOUSE_NONE = 0x0000,
            INTERCEPTION_FILTER_MOUSE_ALL = 0xFFFF,

            INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
            INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
            INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,

            INTERCEPTION_FILTER_MOUSE_BUTTON_1_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_1_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_1_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_1_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_2_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_2_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_2_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_2_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_3_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_3_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_3_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_3_UP,

            INTERCEPTION_FILTER_MOUSE_BUTTON_4_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_4_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_4_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_4_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_5_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_5_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_5_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_5_UP,

            INTERCEPTION_FILTER_MOUSE_WHEEL = InterceptionMouseState.INTERCEPTION_MOUSE_WHEEL,
            INTERCEPTION_FILTER_MOUSE_HWHEEL = InterceptionMouseState.INTERCEPTION_MOUSE_HWHEEL,

            INTERCEPTION_FILTER_MOUSE_MOVE = 0x1000
        }

        public enum InterceptionMouseFlag
        {
            INTERCEPTION_MOUSE_MOVE_RELATIVE = 0x000,
            INTERCEPTION_MOUSE_MOVE_ABSOLUTE = 0x001,
            INTERCEPTION_MOUSE_VIRTUAL_DESKTOP = 0x002,
            INTERCEPTION_MOUSE_ATTRIBUTES_CHANGED = 0x004,
            INTERCEPTION_MOUSE_MOVE_NOCOALESCE = 0x008,
            INTERCEPTION_MOUSE_TERMSRV_SRC_SHADOW = 0x100
        }

        #endregion
    }
}
