using System.Runtime.InteropServices;
using System.Text;

namespace Clairvoyance
{
    /// <summary>
    /// Wrapping WindowsApi.
    /// </summary>
    public class WinOperator
    {
        private IntPtr hWnd;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public WinOperator()
        {
            IntPtr hWnd = GetForegroundWindow();
            StringBuilder className = new StringBuilder(256);
            GetClassName(hWnd, className, className.Capacity);
            string cls = className.ToString();
            if (cls == "CabinetWClass" || cls == "ExploreWClass")
            {
                this.hWnd = hWnd;
            }
            else
            {
                throw new Exception();
            }
        }

        public IntPtr GetHWnd()
        {
            return this.hWnd;
        }

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOP = new IntPtr(0);
        const uint SWP_NOACTIVATE = 0x0010;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;

        public void FollowParent(IntPtr childHWnd, Rect rect)
        {
            POINT pt = new POINT { X = (int)rect.X, Y = (int)rect.Y };
            ScreenToClient(this.hWnd, ref pt);

            SetParent(childHWnd, this.hWnd);
            SetWindowPos(childHWnd, IntPtr.Zero, pt.X, pt.Y, (int)rect.Width, (int)rect.Height, SWP_NOACTIVATE);
        }
    }
}
