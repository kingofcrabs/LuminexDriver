using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace LuminexController
{
    class WindowMessenger
    {
        #region nativeAPI
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out  Rect lpRect);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

      

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr HWnd, uint Msg, int WParam, int LParam);

        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        private static extern int PostMessage(IntPtr HWnd, uint Msg, int WParam, int LParam);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(
            IntPtr hWnd // handle to window
            );


        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SETTEXT = 0x000C;
        private const int WM_CHAR = 0x0102;
        private const int VK_RETURN = 0x0D;
        private const int VK_UP = 0x26;
        private const int VK_CONTROL = 0x11;

        private const int SC_MINIMIZE = 61472;
        public const int WM_SYSCOMMAND = 0x112;
        public const int SC_MAXIMIZE = 0xF030;
        public const int KEYEVENTF_KEYDOWN = 0x0000; //Key down flag
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //ext key flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const byte A = 0x41; //A Control key code
        public const byte C = 0x43; //A Control key code
        public const byte O = 0x4F;
        public const byte N = 0x4E;
        #endregion

        internal void Click()
        {
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        internal void MouseMove(int x, int y)
        {
            SetCursorPos(x, y);
        }

      

        internal POINT GetWindowCenter(SystemWindow window)
        {
            int x = window.Rectangle.Left + window.Rectangle.Width / 2;
            int y = window.Rectangle.Top + window.Rectangle.Height / 2;
            return new POINT(x, y);
        }

        public void MaximizeWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
        }
        public void ReleaseKey()
        {
            keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        internal void Open()
        {
            keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
            //keybd_event(O, 0, KEYEVENTF_KEYDOWN, 0); 
            //keybd_event(O, 0, KEYEVENTF_KEYUP, 0);
            //keybd_event(N, 0, KEYEVENTF_KEYDOWN, 0); 
            //keybd_event(N, 0, KEYEVENTF_KEYUP, 0);

            keybd_event(O, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(O, 0, KEYEVENTF_KEYUP, 0);


            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);


            //INPUT[] inDown = new INPUT[4];
            //inDown[0] = new INPUT();
            //inDown[1] = new INPUT();
            //inDown[2] = new INPUT();
            //inDown[3] = new INPUT();


            //inDown[0].type = inDown[1].type = inDown[2].type = inDown[3].type = INPUT_KEYBOARD;
            //inDown[0].ki.wVk = inDown[2].ki.wVk = (int)Keys.LControlKey;
            //inDown[1].ki.wVk = inDown[3].ki.wVk = (int)Keys.O;
            //inDown[2].ki.dwFlags = inDown[3].ki.dwFlags = KEYEVENTF_KEYUP;
            //SendInput(4, inDown, Marshal.SizeOf(inDown[0]));

        }

    }
}
