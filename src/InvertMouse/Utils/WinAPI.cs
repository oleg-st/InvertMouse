using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace InvertMouse.Utils
{
    static class WinAPI
    {
        private const string Kernel32 = "kernel32.dll";
        private const string User32 = "user32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        [DllImport(Kernel32, SetLastError = true)]
        static extern bool SetPriorityClass(IntPtr handle, PriorityClass priorityClass);

        [DllImport(Kernel32, SetLastError = true)]
        static extern IntPtr GetCurrentProcess();

        [DllImport(User32, SetLastError = true)]
        public static extern bool GetCursorInfo(ref CURSORINFO pci);

        [DllImport(Kernel32)]
        public static extern uint GetLastError();

        public static void RaiseProcessPriority()
        {
            SetPriorityClass(GetCurrentProcess(), PriorityClass.HIGH_PRIORITY_CLASS);
        }

        public const int CURSOR_SHOWING = 0x00000001;
        public const int CURSOR_SUPPRESSED = 0x00000002;

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        #endregion

        #region Enums

        enum PriorityClass : uint
        {
            ABOVE_NORMAL_PRIORITY_CLASS = 0x8000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x4000,
            HIGH_PRIORITY_CLASS = 0x80,
            IDLE_PRIORITY_CLASS = 0x40,
            NORMAL_PRIORITY_CLASS = 0x20,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x100000,
            PROCESS_MODE_BACKGROUND_END = 0x200000,
            REALTIME_PRIORITY_CLASS = 0x100
        }

        #endregion
    }
}
