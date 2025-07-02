using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

        [DllImport(User32)]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport(User32)]
        public static extern short GetKeyState(Keys vKey);


        [DllImport(User32)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport(User32)]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags,
            IntPtr dwhkl);

        [DllImport(User32)]
        public static extern IntPtr GetKeyboardLayout(int idThread);

        public const int CURSOR_SHOWING = 0x00000001;
        public const int CURSOR_SUPPRESSED = 0x00000002;

        private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
        private const int PROCESSOR_ARCHITECTURE_ARM = 5;
        private const int PROCESSOR_ARCHITECTURE_ARM64 = 12;
        private const int PROCESSOR_ARCHITECTURE_IA64 = 6;
        private const int PROCESSOR_ARCHITECTURE_INTEL = 0;
        private const int PROCESSOR_ARCHITECTURE_UNKNOWN = 0xffff;

        [DllImport(Kernel32)]
        public static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        public static string GetArchitecture()
        {
            var sysInfo = new SYSTEM_INFO();
            GetNativeSystemInfo(ref sysInfo);

            switch (sysInfo.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_AMD64:
                    return "x64";
                case PROCESSOR_ARCHITECTURE_ARM64:
                    return "arm64";
                case PROCESSOR_ARCHITECTURE_ARM:
                    return "arm";
                case PROCESSOR_ARCHITECTURE_IA64:
                    return "ia64";
                case PROCESSOR_ARCHITECTURE_INTEL:
                    return "x86";
                default:
                    return null;
            }
        }

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

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
