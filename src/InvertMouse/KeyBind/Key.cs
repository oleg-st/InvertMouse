using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using InvertMouse.Utils;
using Newtonsoft.Json;

namespace InvertMouse.KeyBind
{
    public struct Key : IEquatable<Key>
    {
        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public Keys KeyCode { get; set; }

        public static readonly Keys[] ModifierKeys = {Keys.ControlKey, Keys.ShiftKey, Keys.Menu};

        [JsonIgnore]
        public bool HasKey => KeyCode != Keys.None;

        private static readonly Dictionary<Keys, string> KeyNames = new Dictionary<Keys, string>
        {
            {Keys.None, ""},
            {Keys.Tab, "Tab"},
            {Keys.Cancel, "Break"},
            {Keys.Back, "Backspace"},
            {Keys.Enter, "Enter"},
            {Keys.ShiftKey, "Shift"},
            {Keys.ControlKey, "Ctrl"},
            {Keys.Menu, "Alt"},
            {Keys.Pause, "Pause"},
            {Keys.CapsLock, "CapsLock"},
            {Keys.Escape, "Esc"},
            {Keys.Space, "Space"},
            {Keys.PageUp, "PageUp"},
            {Keys.PageDown, "PageDown"},
            {Keys.End, "End"},
            {Keys.Home, "Home"},
            {Keys.Insert, "Ins"},
            {Keys.Delete, "Del"},
            {Keys.Sleep, "Sleep"},
            {Keys.Multiply, "NumPad*"},
            {Keys.Add, "NumPad+"},
            {Keys.Subtract, "NumPad-"},
            {Keys.Decimal, "NumPad."},
            {Keys.Divide, "NumPad/"},
            {Keys.F1, "F1"},
            {Keys.F2, "F2"},
            {Keys.F3, "F3"},
            {Keys.F4, "F4"},
            {Keys.F5, "F5"},
            {Keys.F6, "F6"},
            {Keys.F7, "F7"},
            {Keys.F8, "F8"},
            {Keys.F9, "F9"},
            {Keys.F10, "F10"},
            {Keys.F11, "F11"},
            {Keys.F12, "F12"},
            {Keys.F13, "F13"},
            {Keys.F14, "F14"},
            {Keys.F15, "F15"},
            {Keys.F16, "F16"},
            {Keys.F17, "F17"},
            {Keys.F18, "F18"},
            {Keys.F19, "F19"},
            {Keys.F20, "F20"},
            {Keys.F21, "F21"},
            {Keys.F22, "F22"},
            {Keys.F23, "F23"},
            {Keys.F24, "F24"},
            {Keys.NumLock, "NumLock"},
            {Keys.Scroll, "ScrollLock"},
            {Keys.LShiftKey, "LShift"},
            {Keys.RShiftKey, "RShift"},
            {Keys.LControlKey, "LCtrl"},
            {Keys.RControlKey, "RCtrl"},
            {Keys.LMenu, "LAlt"},
            {Keys.RMenu, "RAlt"},
            {Keys.Play, "Play"},
            {Keys.Zoom, "Zoom"},
        };

        private static bool IsKeyPressed(Keys key) => (WinAPI.GetKeyState(key) & 0x8000) != 0;

        private static bool IsKeyWasPressed(Keys key) => (WinAPI.GetAsyncKeyState(key) & 0x1) != 0;


        public override string ToString()
        {
            return GetName();
        }

        public void ResetAsyncState()
        {
            if (HasKey)
            {
                IsKeyWasPressed(KeyCode);
            }
        }

        public bool IsWasPressed()
        {
            return HasKey &&
                   IsKeyWasPressed(KeyCode) &&
                   (!Control || IsKeyPressed(Keys.ControlKey)) &&
                   (!Shift || IsKeyPressed(Keys.ShiftKey)) &&
                   (!Alt || IsKeyPressed(Keys.Menu));
        }

        public string GetName()
        {
            var bindName = "";

            if (Control)
            {
                bindName += "Ctrl+";
            }

            if (Alt)
            {
                bindName += "Alt+";
            }

            if (Shift)
            {
                bindName += "Shift+";
            }

            string keyName;
            if (KeyCode == Keys.None)
            {
                keyName = "";
            }
            else
            {
                if (!KeyNames.TryGetValue(KeyCode, out keyName))
                {
                    var vkKeyName = VkCodeToUnicode((uint) KeyCode);
                    keyName = !string.IsNullOrEmpty(vkKeyName) ? vkKeyName : KeyCode.ToString();
                }
            }

            return bindName + keyName;
        }

        private static string VkCodeToUnicode(uint vkCode)
        {
            var sbString = new StringBuilder(16);
            var bKeyState = new byte[255];
            bKeyState[(int) Keys.CapsLock] = 1;
            var lScanCode = WinAPI.MapVirtualKey(vkCode, 0);
            return WinAPI.ToUnicodeEx(vkCode, lScanCode, bKeyState, sbString, sbString.Capacity, 0,
                InputLanguage.DefaultInputLanguage.Handle) <= 0
                ? ""
                : sbString.ToString();
        }

        public void Clear()
        {
            KeyCode = Keys.None;
            Control = Alt = Shift = false;
        }

        public static bool operator ==(Key lhs, Key rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Key lhs, Key rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Key other)
        {
            return Alt == other.Alt && Control == other.Control && Shift == other.Shift && KeyCode == other.KeyCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Key other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Alt.GetHashCode();
                hashCode = (hashCode * 397) ^ Control.GetHashCode();
                hashCode = (hashCode * 397) ^ Shift.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) KeyCode;
                return hashCode;
            }
        }
    }
}