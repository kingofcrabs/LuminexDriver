using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LCLuminexController
{
    class CaptureHelper
    {


        #region 使用BitBlt方法抓取控件，无论控件是否被遮挡
        /// <summary>
        /// 控件(窗口)的截图，控件被其他窗口(而非本窗口内控件)遮挡时也可以正确截图，使用BitBlt方法
        /// </summary>
        /// <param name="control">需要被截图的控件</param>
        /// <returns>该控件的截图，控件被遮挡时也可以正确截图</returns>
        public static Bitmap CaptureControl(IntPtr handle)
        {
            //调用API截屏
            IntPtr hSrce = GetWindowDC(handle);
            IntPtr hDest = CreateCompatibleDC(hSrce);
            int width = 919 - 262;
            int height = 506 - 225;
            IntPtr hBmp = CreateCompatibleBitmap(hSrce, width, height);
            IntPtr hOldBmp = SelectObject(hDest, hBmp);
            if (BitBlt(hDest, 0, 0, width, height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt))
            {
                Bitmap bmp = Image.FromHbitmap(hBmp);
                SelectObject(hDest, hOldBmp);
                DeleteObject(hBmp);
                DeleteDC(hDest);
                ReleaseDC(handle, hSrce);
                return bmp;
            }
            return null;

        }
        #endregion

        #region DLL calls
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        #endregion
    }
}