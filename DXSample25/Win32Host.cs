using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace DXSample25
{
    public class Win32Host : HwndHost
    {
        // ReSharper disable InconsistentNaming
        private const Int32 GWL_STYLE = -16;

        /*publicprivate const Int32 GWL_EXSTYLE = -20;*/

        private const Int32 WS_CAPTION = 0x00C00000;

        private const Int32 WS_THICKFRAME = 0x00040000;

        private const Int32 WS_CHILD = 0x40000000;

        /*private const Int32 WM_PAINT = 0x0000000F;*/
        /*private const Int32 WS_EX_TOPMOST = 0x00000008;*/
        // ReSharper restore InconsistentNaming
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern Int32 SetWindowLong(IntPtr hWnd, Int32 gwl, Int32 style);

        [DllImport("user32.dll")]
        static extern Int32 GetWindowLong(IntPtr hWnd, Int32 gwl);

        private IntPtr ChildHandle { get; }

        public Win32Host(IntPtr handle)
        {
            ChildHandle = handle;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HandleRef href = new HandleRef();

            if (ChildHandle != IntPtr.Zero)
            {
                SetWindowLong(ChildHandle, GWL_STYLE, ((GetWindowLong(ChildHandle, GWL_STYLE) | WS_CHILD) & ~WS_CAPTION) & ~WS_THICKFRAME);
                SetChildParent(hwndParent);
                href = new HandleRef(this, ChildHandle);
            }

            return href;
        }

        private void SetChildParent(HandleRef hwndParent)
        {
            SetParent(ChildHandle, hwndParent.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            SetChildParent(new HandleRef(this, IntPtr.Zero));
        }
    }
}