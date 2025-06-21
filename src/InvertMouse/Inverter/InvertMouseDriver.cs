using InvertMouse.Utils;
using System;
using System.Threading;

namespace InvertMouse.Inverter
{
    /*
     * Using InvertMouse driver
     */
    public class InvertMouseDriver: InvertMouseBase
    {
        private bool? _currentActive;
        private decimal? _xMultiplierCurrent, _yMultiplierCurrent;
        private readonly InvertMouseControl _invertMouseControl = new InvertMouseControl();

        public InvertMouseDriver()
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
            while (IsRunning)
            {
                UpdateActive(!WhenCursorIsHidden || IsCursorHidden());
                Thread.Sleep(50);
            }
        }

        private void UpdateActive(bool active)
        {
            var xMultiplier = XMultiplier;
            var yMultiplier = YMultiplier;
            if (_currentActive != active || xMultiplier != _xMultiplierCurrent || yMultiplier != _yMultiplierCurrent)
            {
                var newSettings = new InvertMouseControl.INVERTMOUSE_SETTINGS
                {
                    enable = active,
                    multiplier_x = (double) xMultiplier,
                    multiplier_y = (double) yMultiplier,
                };

                _invertMouseControl.SetSettings(newSettings);
                _currentActive = active;
                _xMultiplierCurrent = xMultiplier;
                _yMultiplierCurrent = yMultiplier;
            }
        }

        private void Check()
        {
            Error = "";
            if (!_invertMouseControl.Open() || !_invertMouseControl.GetSettings(out _))
            {
                State = CheckState.DriverNotInstalled;
                return;
            }

            State = CheckState.Ok;
        }

        public override void Stop()
        {
            base.Stop();
            UpdateActive(false);
        }

        public override string Name => "InvertMouse";
    }
}
