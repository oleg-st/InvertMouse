using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace InvertMouse.KeyBind
{
    public class KeyBindManager
    {
        private Thread _thread;

        private Key _bindKey;

        public bool IsSuspended { get; private set; }
        public bool IsRunning { get; private set; }

        public event EventHandler Fire;

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

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            lock (this)
            {
                _thread = new Thread(Worker);
                IsRunning = true;
                _thread.Start();
            }
        }

        public void Suspend()
        {
            IsSuspended = true;
        }

        public void Resume()
        {
            IsSuspended = false;
        }

        private void Worker()
        {
            _bindKey.ResetAsyncState();

            while (IsRunning)
            {
                if (!IsSuspended && _bindKey.IsWasPressed())
                {
                    Trace.WriteLine("FIRE");
                    Fire?.Invoke(this, EventArgs.Empty);
                }

                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            lock (this)
            {
                _thread.Join();
            }
        }

        public void Bind(Key key)
        {
            key.ResetAsyncState();
            _bindKey = key;
        }
    }
}
