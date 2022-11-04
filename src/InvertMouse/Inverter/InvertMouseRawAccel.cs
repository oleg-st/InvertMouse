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
        private decimal _xMultiplierCurrent = IdentityMultiplier, _yMultiplierCurrent = IdentityMultiplier;

        private Profile _profile;
        private DriverConfig _driverConfig;

        private double _baseSensitivity;
        private double _baseSensYxRatio;

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

            while (IsRunning)
            {
                UpdateActive(!WhenCursorIsHidden || IsCursorHidden());
                Thread.Sleep(50);
            }
        }

        private void ApplyInvert(decimal xMultiplier, decimal yMultiplier)
        {
            if (_profile == null)
            {
                var activeDriverConfig = DriverConfig.GetActive();
                _profile = activeDriverConfig.profiles.FirstOrDefault() ?? new Profile();
                _driverConfig = DriverConfig.FromProfile(_profile);
                _baseSensitivity = _profile.sensitivity;
                _baseSensYxRatio = _profile.yxSensRatio;
            }

            _profile.sensitivity = _baseSensitivity * (double) xMultiplier;
            _profile.yxSensRatio = _baseSensYxRatio * (double) (yMultiplier / xMultiplier);
            _driverConfig.SetProfileAt(0, _profile);
            _driverConfig.Activate();
        }

        private void UpdateActive(bool active)
        {
            Update(active ? XMultiplier : IdentityMultiplier, active ? YMultiplier : IdentityMultiplier);
        }

        private void Update(decimal xMultiplier, decimal yMultiplier)
        {
            if (_xMultiplierCurrent != xMultiplier|| _yMultiplierCurrent != yMultiplier)
            {
                ApplyInvert(xMultiplier, yMultiplier);
                _xMultiplierCurrent = xMultiplier;
                _yMultiplierCurrent = yMultiplier;
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
