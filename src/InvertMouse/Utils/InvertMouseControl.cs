using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// ReSharper disable InconsistentNaming

namespace InvertMouse.Utils
{
    internal class InvertMouseControl : IDisposable
    {
        const string DevicePath = @"\\.\invertmouse";

        const uint FILE_DEVICE_MOUSE = 0x0000000f;
        const uint METHOD_BUFFERED = 0;
        const uint FILE_ANY_ACCESS = 0;
        static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
            => ((deviceType << 16) | (access << 14) | (function << 2) | method);

        static readonly uint IOCTL_INVERTMOUSE_GET_SETTINGS =
            CTL_CODE(FILE_DEVICE_MOUSE, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static readonly uint IOCTL_INVERTMOUSE_SET_SETTINGS =
            CTL_CODE(FILE_DEVICE_MOUSE, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static readonly uint IOCTL_INVERTMOUSE_GET_VERSION =
            CTL_CODE(FILE_DEVICE_MOUSE, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS);

        [StructLayout(LayoutKind.Sequential)]
        public struct INVERTMOUSE_SETTINGS
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool enable;
            public double multiplier_x;
            public double multiplier_y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INVERTMOUSE_VERSION
        {
            public int major;
            public int minor;
            public int build;
            public int revision;
        }

        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport(Kernel32, SetLastError = true)]
        static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr inBuffer,
            int nInBufferSize,
            [Out] out INVERTMOUSE_SETTINGS outBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        [DllImport(Kernel32, SetLastError = true)]
        static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr inBuffer,
            int nInBufferSize,
            [Out] out INVERTMOUSE_VERSION outBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        [DllImport(Kernel32, SetLastError = true)]
        static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            [In] ref INVERTMOUSE_SETTINGS inBuffer,
            int nInBufferSize,
            IntPtr outBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint OPEN_EXISTING = 3;
        const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        private SafeFileHandle _controlDevice;

        public bool Open()
        {
            var device = CreateFile(DevicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (device.IsInvalid)
            {
                _controlDevice = null;
                device.Dispose();
                return false;
            }

            _controlDevice = device;
            return true;
        }

        public bool IsOpened => _controlDevice != null;

        public bool GetSettings(out INVERTMOUSE_SETTINGS settings)
        {
            if (_controlDevice == null)
            {
                settings = default;
                return false;
            }

            var size = Marshal.SizeOf<INVERTMOUSE_SETTINGS>();
            return DeviceIoControl(_controlDevice, IOCTL_INVERTMOUSE_GET_SETTINGS,
                IntPtr.Zero, 0,
                out settings, size,
                out var returned, IntPtr.Zero) && returned == size;
        }

        public bool GetVersion(out INVERTMOUSE_VERSION version)
        {
            if (_controlDevice == null)
            {
                version = default;
                return false;
            }

            var size = Marshal.SizeOf<INVERTMOUSE_VERSION>();
            return DeviceIoControl(_controlDevice, IOCTL_INVERTMOUSE_GET_VERSION,
                IntPtr.Zero, 0,
                out version, size,
                out var returned, IntPtr.Zero) && returned == size;
        }

        public bool SetSettings(INVERTMOUSE_SETTINGS settings)
        {
            return _controlDevice != null && DeviceIoControl(_controlDevice, IOCTL_INVERTMOUSE_SET_SETTINGS,
                ref settings, Marshal.SizeOf<INVERTMOUSE_SETTINGS>(),
                IntPtr.Zero, 0,
                out _, IntPtr.Zero);
        }

        public void Dispose()
        {
            _controlDevice?.Dispose();
        }
    }
}
