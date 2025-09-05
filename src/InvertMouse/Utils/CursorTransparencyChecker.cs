using System;
using System.Runtime.InteropServices;

namespace InvertMouse.Utils
{
    internal unsafe class CursorTransparencyChecker : IDisposable
    {
        private IntPtr _hdc = IntPtr.Zero;
        private byte[] _buffer;

        private void EnsureHdc()
        {
            if (_hdc == IntPtr.Zero)
            {
                _hdc = WinAPI.CreateCompatibleDC(IntPtr.Zero);
            }
        }

        private void EnsureBuffer(int w, int h)
        {
            // at least 32 bits for pixel
            var need = w * h * 4;
            if (_buffer == null || _buffer.Length < need)
            {
                _buffer = new byte[need];
            }
        }

        public bool IsCursorFullyTransparent(IntPtr hCursor)
        {
            if (hCursor == IntPtr.Zero || !WinAPI.GetIconInfo(hCursor, out var iconInfo))
            {
                return false;
            }

            try
            {
                if (iconInfo.hbmColor != IntPtr.Zero)
                {
                    // color cursor
                    WinAPI.BITMAP bm = default;
                    WinAPI.GetObject(iconInfo.hbmColor, Marshal.SizeOf<WinAPI.BITMAP>(), ref bm);
                    var w = bm.bmWidth;
                    var h = bm.bmHeight;
                    // empty cursor
                    if (w <= 0 || h <= 0)
                    {
                        return true;
                    }

                    // 32 bits -> only ALPHA
                    if (bm.bmBitsPixel == 32)
                    {
                        if (!GetBitmapBytes(iconInfo.hbmColor, w, h, 32))
                        {
                            return true;
                        }

                        // ALPHA = 0
                        fixed (byte* pStart = _buffer)
                        {
                            var pEnd = pStart + w * h * 4;
                            for (var p = pStart; p < pEnd; p += 4)
                            {
                                if (p[3] != 0)
                                {
                                    return false;
                                }
                            }
                        }

                        return true;
                    }

                    if (iconInfo.hbmMask == IntPtr.Zero || !GetBitmapBytes(iconInfo.hbmMask, w, h, 1))
                    {
                        return false;
                    }

                    // AND = 1
                    fixed (byte* andMask = _buffer)
                    {
                        if (!AllBitsOne(andMask, w, h))
                        {
                            return false;
                        }
                    }

                    if (!GetBitmapBytes(iconInfo.hbmColor, w, h, 24))
                    {
                        return false;
                    }

                    // COLOR = 0
                    fixed (byte* pStart = _buffer)
                    {
                        var pEnd = pStart + w * h * 3;
                        for (var p = pStart; p < pEnd; p++)
                        {
                            if (*p != 0)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    // monochrome cursor
                    WinAPI.BITMAP bm = default;
                    WinAPI.GetObject(iconInfo.hbmMask, Marshal.SizeOf<WinAPI.BITMAP>(), ref bm);
                    var w = bm.bmWidth;
                    var h2 = bm.bmHeight;
                    // empty cursor
                    if (w <= 0 || h2 <= 0 || GetBitmapBytes(iconInfo.hbmMask, w, h2, 1))
                    {
                        return true;
                    }

                    var h = h2 / 2;
                    fixed (byte* andMask = _buffer)
                    {
                        // AND = 1
                        if (!AllBitsOne(andMask, w, h))
                        {
                            return false;
                        }
                    }
                    var maskStride = (w + 31) / 32 * 4;
                    fixed (byte* xorMask = &_buffer[maskStride * h])
                    {
                        // XOR = 0
                        if (!AllBitsZero(xorMask, w, h))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            finally
            {
                if (iconInfo.hbmMask != IntPtr.Zero)
                {
                    WinAPI.DeleteObject(iconInfo.hbmMask);
                }

                if (iconInfo.hbmColor != IntPtr.Zero)
                {
                    WinAPI.DeleteObject(iconInfo.hbmColor);
                }
            }
        }

        private bool GetBitmapBytes(IntPtr hBmp, int w, int h, ushort bitCount)
        {
            EnsureHdc();
            if (_hdc == IntPtr.Zero)
            {
                return false;
            }

            WinAPI.BITMAPINFO bmi = default;
            bmi.bmiHeader.biSize = (uint)Marshal.SizeOf<WinAPI.BITMAPINFOHEADER>();
            bmi.bmiHeader.biWidth = w;
            bmi.bmiHeader.biHeight = -h;
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = bitCount;
            bmi.bmiHeader.biCompression = WinAPI.BI_RGB;

            var old = WinAPI.SelectObject(_hdc, hBmp);
            try
            {
                EnsureBuffer(w, h);
                return WinAPI.GetDIBits(_hdc, hBmp, 0, (uint)h, _buffer, ref bmi, WinAPI.DIB_RGB_COLORS) != 0;
            }
            finally
            {
                WinAPI.SelectObject(_hdc, old);
            }
        }

        private static bool AllBitsOne(byte* buf, int width, int height)
        {
            var stride = (width + 31) / 32 * 4;
            var fullBytes = width >> 3;
            var tailBits = width & 7;

            var pEnd = buf + height * stride;
            for (var p = buf; p < pEnd; p += stride)
            {
                for (var i = 0; i < fullBytes; i++)
                {
                    if (p[i] != 0xFF)
                    {
                        return false;
                    }
                }

                if (tailBits != 0)
                {
                    var tail = p[fullBytes];
                    var needed = 0xFF << (8 - tailBits);
                    if ((tail & needed) != needed)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool AllBitsZero(byte* buf, int width, int height)
        {
            var stride = (width + 31) / 32 * 4;
            var fullBytes = width >> 3;
            var tailBits = width & 7;

            var pEnd = buf + height * stride;
            for (var p = buf; p < pEnd; p += stride)
            {
                for (var i = 0; i < fullBytes; i++)
                {
                    if (p[i] != 0)
                    {
                        return false;
                    }
                }

                if (tailBits != 0)
                {
                    var tail = p[fullBytes];
                    var needed = 0xFF << (8 - tailBits);
                    if ((tail & needed) != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Dispose()
        {
            if (_hdc != IntPtr.Zero)
            {
                WinAPI.DeleteDC(_hdc);
                _hdc = IntPtr.Zero;
            }

            _buffer = null;
        }
    }
}
