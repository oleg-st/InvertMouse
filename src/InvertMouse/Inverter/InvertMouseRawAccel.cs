using InvertMouse.Utils;
using System;
using System.Linq;
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

        private Profile _profile;
        private DriverConfig _driverConfig;

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
            if (_profile == null)
            {
                var activeDriverConfig = DriverConfig.GetActive();
                _profile = activeDriverConfig.profiles.FirstOrDefault() ?? new Profile();
                _driverConfig = DriverConfig.FromProfile(_profile);
            }

            _profile.sensitivity = Math.Abs(_profile.sensitivity) * (xAxis ? -1 : 1);
            _profile.yxSensRatio = Math.Abs(_profile.yxSensRatio) * (yAxis != xAxis ? -1 : 1);
            _driverConfig.SetProfileAt(0, _profile);
            _driverConfig.Activate();
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
                VersionHelper.ValidOrThrow();
            }
            catch (InteropException ex)
            {
                State = CheckState.DriverNotInstalled;
                Error = ex.Message;
                return;
            }

            State = CheckState.Ok;
            Delay = DriverConfig.WriteDelayMs;
        }

        public override void Stop()
        {
            base.Stop();
            UpdateActive(false);
        }

        public override string Name => "RawAccel";
    }
}
