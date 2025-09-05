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
        private const string Gdi32 = "gdi32.dll";

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

        public const int DIB_RGB_COLORS = 0;
        public const int BI_RGB = 0;

        [DllImport(User32)]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
        [DllImport(Gdi32)]
        public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, ref BITMAP lpvObject);
        [DllImport(Gdi32)] 
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport(Gdi32)]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport(Gdi32)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport(Gdi32)]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref BITMAPINFO lpbi, uint uUsage);
        [DllImport(Gdi32, SetLastError = false)]
        public static extern bool DeleteObject(IntPtr hObject);

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

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            [MarshalAs(UnmanagedType.Bool)] public bool fIcon;
            public int xHotspot, yHotspot;
            public IntPtr hbmMask, hbmColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType, bmWidth, bmHeight, bmWidthBytes;
            public ushort bmPlanes, bmBitsPixel;
            public IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;   // >0 = bottom-up
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO { public BITMAPINFOHEADER bmiHeader; }
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
