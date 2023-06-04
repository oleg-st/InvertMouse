using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InvertMouse.KeyBind
{
    public class KeyBinder
    {
        private bool _isBinding;

        private Key _currentKey;

        public Key CurrentKey => _currentKey;

        public event EventHandler Start;
        public event EventHandler End;
        public event EventHandler Update;
        public event EventHandler Cancel;

        private readonly HashSet<Keys> _keysPressed = new HashSet<Keys>();

        private Keys ButtonToKey(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return Keys.LButton;
                case MouseButtons.Middle:
                    return Keys.MButton;
                case MouseButtons.Right:
                    return Keys.RButton;
                case MouseButtons.XButton1:
                    return Keys.XButton1;
                case MouseButtons.XButton2:
                    return Keys.XButton2;
                default:
                    return Keys.None;
            }
        }

        private bool IsKeyModifier(Keys keyCode) => Key.ModifierKeys.Contains(keyCode);

        public void KeyDown(Keys keyCode)
        {
            if (keyCode == Keys.Escape)
            {
                _currentKey.Clear();
                EndKeyBind();
                Cancel?.Invoke(this, EventArgs.Empty);
                return;
            }

            _keysPressed.Add(keyCode);

            if (!IsKeyModifier(keyCode))
            {
                StartKeyBind();
                _currentKey.KeyCode = keyCode;
            }
            else if(!_isBinding)
            {
                StartKeyBind();
            }

            UpdateKey();
        }

        public void KeyUp(Keys keyCode)
        {
            if (keyCode == Keys.Escape)
            {
                return;
            }

            _keysPressed.Remove(keyCode);

            if (!_keysPressed.Any(IsKeyModifier) && (!_currentKey.HasKey || !_keysPressed.Contains(_currentKey.KeyCode)))
            {
                EndKeyBind();
            }
            else if (!_currentKey.HasKey)
            {
                UpdateKey();
            }
        }

        public void MouseDown(MouseButtons button)
        {
            var key = ButtonToKey(button);
            if (key != Keys.None && button != MouseButtons.Left)
            {
                KeyDown(key);
            }
        }

        public void MouseUp(MouseButtons button)
        {
            var key = ButtonToKey(button);
            if (key != Keys.None && button != MouseButtons.Left)
            {
                KeyUp(key);
            }
        }

        private void UpdateKey()
        {
            _currentKey.Control = _keysPressed.Contains(Keys.ControlKey);
            _currentKey.Alt = _keysPressed.Contains(Keys.Menu);
            _currentKey.Shift = _keysPressed.Contains(Keys.ShiftKey);
            Update?.Invoke(this, EventArgs.Empty);
        }

        private void StartKeyBind()
        {
            _currentKey.Clear();
            _isBinding = true;
            Start?.Invoke(this, EventArgs.Empty);
        }

        private void EndKeyBind()
        {
            if (_isBinding)
            {
                _isBinding = false;
                End?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
