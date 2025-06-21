using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace DriverInstaller.Utils
{
    internal class DriverInstaller
    {
        // WinAPI constants and imports
        private const uint SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const uint SERVICE_ALL_ACCESS = 0xF01FF;
        private const uint SERVICE_KERNEL_DRIVER = 0x00000001;
        private const uint SERVICE_DEMAND_START = 0x00000003;
        private const uint SERVICE_ERROR_NORMAL = 0x00000001;
        private const uint ERROR_SERVICE_DOES_NOT_EXIST = 1060;

        private const string AdvApi32 = "advapi32.dll";
        private const string Kernel32 = "kernel32.dll";

        [DllImport(AdvApi32, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport(AdvApi32, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateService(
            IntPtr hSCManager, string lpServiceName, string lpDisplayName,
            uint dwDesiredAccess, uint dwServiceType, uint dwStartType, uint dwErrorControl,
            string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId,
            string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport(AdvApi32, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport(AdvApi32, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool ChangeServiceConfig(
            IntPtr hService, uint nServiceType, uint nStartType, uint nErrorControl,
            string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId,
            string lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        [DllImport(AdvApi32, SetLastError = true)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

        private const int MOVEFILE_REPLACE_EXISTING = 0x1;
        private const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;

        public const string DriverName = "InvertMouse";
        public const string DriverFileName = DriverName + ".sys";
        private readonly string DriverDestPath = Path.Combine(Environment.SystemDirectory, "drivers", DriverFileName);

        private const string SetupApi = "setupapi.dll";
        private const uint SPCRP_UPPERFILTERS = 0x00000011;

        private static readonly Guid GUID_DEVCLASS_MOUSE =
            new Guid(0x4d36e96f, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);

        [DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool SetupDiGetClassRegistryPropertyW(
            [In] ref Guid ClassGuid,
            uint Property,
            out uint PropertyRegDataType,
            [Out] byte[] PropertyBuffer,
            uint PropertyBufferSize,
            out uint RequiredSize,
            IntPtr MachineName,
            IntPtr Reserved2);

        [DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool SetupDiSetClassRegistryPropertyW(
            [In] ref Guid ClassGuid,
            uint Property,
            byte[] PropertyBuffer,
            uint PropertyBufferSize,
            IntPtr MachineName,
            IntPtr Reserved);

        private const string PsApi = "psapi.dll";

        [DllImport(PsApi, SetLastError = true)]
        static extern bool EnumDeviceDrivers(
            [Out] IntPtr[] lpImageBase,
            uint cb,
            [Out] out uint lpcbNeeded
        );

        [DllImport(PsApi, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetDeviceDriverBaseName(
            IntPtr ImageBase,
            [Out] StringBuilder lpBaseName,
            uint nSize
        );

        [DllImport(PsApi, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetDeviceDriverFileName(
            IntPtr ImageBase,
            [Out] StringBuilder lpFilename,
            uint nSize
        );

        private void AddService(IntPtr scm)
        {
            var srv = CreateService(
                scm, DriverDestPath, DriverDestPath, SERVICE_ALL_ACCESS, SERVICE_KERNEL_DRIVER,
                SERVICE_DEMAND_START, SERVICE_ERROR_NORMAL, DriverDestPath,
                null, IntPtr.Zero, null, null, null
            );
            if (srv == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateService failed");

            CloseServiceHandle(srv);
        }

        private bool UpdateService(IntPtr srv)
        {
            return ChangeServiceConfig(
                srv, SERVICE_KERNEL_DRIVER, SERVICE_DEMAND_START, SERVICE_ERROR_NORMAL,
                DriverDestPath, null, IntPtr.Zero, null, null, null, DriverName
            );
        }

        public void Install(string driverSourcePath)
        {
            var scm = IntPtr.Zero;
            try
            {
                var osVersion = Environment.OSVersion.Version;
                if (osVersion.Major < 10)
                    throw new Exception("OS not supported, you need at least Windows 10");

                if (!File.Exists(driverSourcePath))
                    throw new Exception("Can't find driver binary");

                if (File.Exists(DriverDestPath))
                {
                    var tmpDestPath = DriverDestPath + ".tmp";
                    if (MoveFileEx(DriverDestPath, tmpDestPath, MOVEFILE_REPLACE_EXISTING))
                        MoveFileEx(tmpDestPath, null, MOVEFILE_DELAY_UNTIL_REBOOT);
                }

                File.Copy(driverSourcePath, DriverDestPath, true);

                scm = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scm == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenSCManager failed");

                var srv = OpenService(scm, DriverName, SERVICE_ALL_ACCESS);
                if (srv != IntPtr.Zero)
                {
                    var success = UpdateService(srv);
                    CloseServiceHandle(srv);
                    if (!success)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "ChangeServiceConfig failed");
                }
                else
                {
                    var error = Marshal.GetLastWin32Error();
                    if (error != ERROR_SERVICE_DOES_NOT_EXIST)
                        throw new Win32Exception(error, "OpenService failed");

                    AddService(scm);
                }

                UpdateRegistry(true);
            }
            finally
            {
                if (scm != IntPtr.Zero)
                {
                    CloseServiceHandle(scm);
                }
            }
        }

        public bool Uninstall()
        {
            var rebootRequired = UpdateRegistry(false);
            var tmp = DriverDestPath + ".tmp";
            if (File.Exists(DriverDestPath))
            {
                rebootRequired = true;

                var renamed = MoveFileEx(DriverDestPath, tmp, MOVEFILE_REPLACE_EXISTING);
                if (renamed)
                {
                    MoveFileEx(tmp, null, MOVEFILE_DELAY_UNTIL_REBOOT);
                }
                else
                {
                    try
                    {
                        File.Delete(DriverDestPath);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }

            return rebootRequired || File.Exists(tmp);
        }

        public bool IsInstalled()
        {
            if (!File.Exists(DriverDestPath))
            {
                return false;
            }

            var value = GetFiltersValue();
            return value != null && GetDriverPosition(value) >= 0;
        }

        private string GetFiltersValue()
        {
            var guid = GUID_DEVCLASS_MOUSE;

            SetupDiGetClassRegistryPropertyW(ref guid, SPCRP_UPPERFILTERS, out _, null, 0,
                out var size, IntPtr.Zero, IntPtr.Zero);

            var buffer = new byte[size];
            if (!SetupDiGetClassRegistryPropertyW(ref guid, SPCRP_UPPERFILTERS, out _, buffer, size, out _, IntPtr.Zero, IntPtr.Zero))
            {
                return null;
            }

            return Encoding.Unicode.GetString(buffer);
        }

        private bool SetFiltersValue(string value)
        {
            var guid = GUID_DEVCLASS_MOUSE;
            var buffer = Encoding.Unicode.GetBytes(value);
            return SetupDiSetClassRegistryPropertyW(ref guid, SPCRP_UPPERFILTERS, buffer, (uint)buffer.Length, IntPtr.Zero, IntPtr.Zero);
        }

        private int GetDriverPosition(string value)
        {
            const string driverNameWithZero = DriverName + "\0";

            if (value.StartsWith(driverNameWithZero))
            {
                return 0;
            }

            var indexOf = value.IndexOf("\0" + driverNameWithZero, StringComparison.InvariantCultureIgnoreCase);
            if (indexOf >= 0)
            {
                return indexOf + 1;
            }

            return -1;
        }

        private bool UpdateRegistry(bool install)
        {
            var stringValue = GetFiltersValue();
            if (stringValue == null)
            {
                throw new Exception("Read registry failed");
            }

            const string driverNameWithZero = DriverName + "\0";

            string newStringValue;

            // remove driver name
            var indexOf = GetDriverPosition(stringValue);
            if (indexOf >= 0)
            {
                newStringValue = stringValue.Substring(0, indexOf) +
                                 stringValue.Substring(indexOf + driverNameWithZero.Length);
            }
            else
            {
                newStringValue = stringValue;
            }

            // add driver name if needed
            if (install)
            {
                newStringValue = driverNameWithZero + newStringValue;
            }

            if (stringValue == newStringValue)
            {
                return false;
            }

            if (!SetFiltersValue(newStringValue))
            {
                throw new Exception("Write registry failed");
            }
            return true;
        }

        public bool IsLoaded()
        {
            EnumDeviceDrivers(Array.Empty<IntPtr>(), 0, out var needed);
            if (needed == 0)
            {
                return false;
            }

            var count = (int)(needed / IntPtr.Size);
            var drivers = new IntPtr[count];

            if (EnumDeviceDrivers(drivers, (uint)(IntPtr.Size * drivers.Length), out var sizeNeeded) && sizeNeeded > 0)
            {
                var buffer = new StringBuilder(1024);
                for (var i = 0; i < count; i++)
                {
                    if (GetDeviceDriverBaseName(drivers[i], buffer, (uint)buffer.Capacity) != 0)
                    {
                        if (buffer.ToString().Equals(DriverFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
