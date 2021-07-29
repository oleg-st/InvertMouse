using InvertMouse.Utils;
using System;
using System.Threading;

namespace InvertMouse.Inverter
{
    /*
     * Using Raw Accel driver
     *
     * https://github.com/a1xd/rawaccel
     */
    public class InvertMouseRawAccel : InvertMouseBase
    {
        private bool _xAxisActive, _yAxisActive;
        // v1.4.4.0
        private static readonly Version WrapperTarget = new Version(1, 4, 4, 0);

        public InvertMouseRawAccel()
        {
            try
            {
                Check();
            }
            catch (Exception ex)
            {
                State = CheckState.LibraryNotLoaded;
                Error = ex.Message;
            }
        }

        protected override void Worker()
        {
            WinAPI.RaiseProcessPriority();

            while (Running)
            {
                UpdateActive(!WhenCursorIsHidden || IsCursorHidden());
                Thread.Sleep(50);
            }
        }

        private void ApplyInvert(bool xAxis, bool yAxis)
        {
            var activeSettings = DriverInterop.GetActiveSettings();
            activeSettings.sensitivity.x = Math.Abs(activeSettings.sensitivity.x) * (xAxis ? -1 : 1);
            activeSettings.sensitivity.y = Math.Abs(activeSettings.sensitivity.y) * (yAxis ? -1 : 1);
            DriverInterop.Write(activeSettings);
        }

        private void UpdateActive(bool active)
        {
            Update(active && XAxis, active && YAxis);
        }

        private void Update(bool xAxisActive, bool yAxisActive)
        {
            if (_xAxisActive != xAxisActive || _yAxisActive != yAxisActive)
            {
                ApplyInvert(xAxisActive, yAxisActive);
                _xAxisActive = xAxisActive;
                _yAxisActive = yAxisActive;
            }
        }

        private void Check()
        {
            Error = "";
            try
            {
                VersionHelper.ValidateAndGetDriverVersion(WrapperTarget);
            }
            catch (VersionException ex)
            {
                State = CheckState.DriverNotInstalled;
                Error = ex.Message;
                return;
            }

            State = CheckState.Ok;
            Delay = DriverInterop.WriteDelayMs;
        }

        public override void Stop()
        {
            base.Stop();
            UpdateActive(false);
        }

        public override string Name => "RawAccel";
    }
}
