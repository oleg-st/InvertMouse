using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace InvertMouse.Utils
{
    public static class AdminManager
    {
        public static bool RestartAsAdministrator()
        {
            var exeName = Process.GetCurrentProcess().MainModule?.FileName;
            var startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas",
                Arguments = Environment.CommandLine,
                UseShellExecute = true
            };
            try
            {
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                // ERROR_CANCELLED
                if (ex is Win32Exception win32Exception && win32Exception.NativeErrorCode == 0x000004C7)
                {
                    return false;
                }

                MessageBox.Show($"Cannot restart the app as administrator: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        public static bool IsAdministrator()
        {
            using (var windowsIdentity = WindowsIdentity.GetCurrent())
                return new WindowsPrincipal(windowsIdentity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
