using LCLuminexController;
using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;



namespace LuminexController
{
    public class WindowOp
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        WindowMessenger winMessenger = new WindowMessenger();
        public List<string> EnumTopWindows()
        {
            return SystemWindow.AllToplevelWindows.Where(x => x.Title != "").Select(x => x.Title).ToList();
        }

        public SystemWindow GetWindow(string winTitle)
        {
            SystemWindow[] selectionWindows = SystemWindow.AllToplevelWindows.Where(x => x.Title.Contains(winTitle)).ToArray();
            if (selectionWindows.Count() == 0)
                return null;
            else
                return selectionWindows[0];
        }

        internal void ClickRun()
        {
            SystemWindow luminexTopWindow = GetLuminexTopWindow();

            if (luminexTopWindow == null)
            {
                throw new Exception("Fail to get luminex top window!");
            }

            IntPtr hwnd = luminexTopWindow.HWnd;
            winMessenger.MaximizeWindow(hwnd);
            //SetForegroundWindow(luminexTopWindow.HWnd);
            AutomationElement mainWindow = AutomationElement.FromHandle(hwnd);
            if (mainWindow == null)
            {
                throw new Exception("Fail to get main Window!");
            }
            LuminexSDK.Instance.Eject();
            mainWindow.SetFocus();
            winMessenger.MouseMove(147, 120);
            winMessenger.Click();
            Thread.Sleep(500);
            winMessenger.MouseMove(667, 544); //start
            winMessenger.Click();
        }

   
        public SystemWindow GetLuminexTopWindow()
        {
            return GetWindow("Luminex100 IS Software");
        }

        protected void Select(AutomationElement element)
        {
            SelectionItemPattern select = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);
            select.Select();
        }

        protected void Click(AutomationElement element)
        {
            var click = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            click.Invoke();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOW = 5;
        const int SW_HIDE = 0;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        internal void HideWindow(SystemWindow window)
        {
            ShowWindow(window.HWnd, SW_HIDE);
        }

        private void MinimizeWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_SHOWMINNOACTIVE);
        }

        private void ShowWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_SHOW);
        }
        
        
        //1 find window
        //2 maximize the window
        //3 bring the window into foreground
        //4 find the open batches window
        //5 capture the image of listview in open batches window
        //6 analysis the listview's image to judge its row's total count
        //7 select the last one.
        //8 click select button
        public bool SelectLastBatch(int batchID)
        {
            bool bok = true;
            try
            {
                SystemWindow luminexTopWindow = GetLuminexTopWindow();

                if (luminexTopWindow == null)
                {
                    Console.WriteLine("Fail to get luminex top window!");

                    return false;
                }
                else
                {
                    Console.WriteLine("Get luminex top window!");

                }

                IntPtr hwnd = luminexTopWindow.HWnd;
                winMessenger.MaximizeWindow(hwnd);
                //SetForegroundWindow(luminexTopWindow.HWnd);
                AutomationElement mainWindow = AutomationElement.FromHandle(hwnd);

                if (mainWindow == null)
                {
                    Console.WriteLine("Fail to get main Window!");

                    return false;
                }
                else
                {
                    Console.WriteLine("Get main Window!");

                }

                try
                {
                    mainWindow.SetFocus();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;

                }
                Thread.Sleep(500); 
                winMessenger.Open(); //send ctrl + o key
                Thread.Sleep(500);
                winMessenger.ReleaseKey(); //release ctrl button

                AutomationElement openBatchWindow = GetOpenBatchesWindow(mainWindow);
                if (openBatchWindow == null)
                {
                    Console.WriteLine("Fail to get open Batch Window!");

                    return false;
                }
                else
                {
                    Console.WriteLine("Get open Batch Window!");

                }
                Bitmap bmp = null;
                AutomationElement lstView = CaptureListView(openBatchWindow, ref  bmp);

                if (lstView == null)
                {
                    Console.WriteLine("Fail to get lstView!");

                    return false;
                }
                int nPosition = GetFirstGrayLinePosition(bmp);

                Console.WriteLine("nPosition is " + nPosition);

                IsValidBatchID(batchID, nPosition);
                ClickLastBatch(lstView, nPosition);

                var selectButton = GetOpenBatchesSelectButton(openBatchWindow);

                Click(selectButton);//another alternative is use ClickButton.
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                bok = false;
            }
            return bok;

        }

        private int GetFirstGrayLinePosition(Bitmap bmp)
        {
            var results = GetTotalGrayLevelsAtDiffY(bmp);
            int grayLinePos = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] == 253040)
                {
                    grayLinePos = i;
                    break;
                }
            }
            return grayLinePos;
        }

        private void IsValidBatchID(int batchID, int nPosition)
        {
            //if not valid, throw expection,listview中的行数，必须>=当前的batchID，行数可以通过第一条灰色线的位置计算出来。
            double nLines = nPosition - 21.0; //first row starts from 21 pixel
            int nRowCnt = (int)Math.Round(nLines / 19.0); //19 pixel per row
            if (batchID > nRowCnt)
                throw new Exception("无法找到当前Batch的定义！");
        }

        private static List<int> GetTotalGrayLevelsAtDiffY(Bitmap bmp)
        {
            BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                      System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                      bmp.PixelFormat);

            List<int> vals = new List<int>();
            int PixelSize = 4;

            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    int val = 0;
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int i = 0; i < 3; i++)
                            val += row[x * PixelSize + i];
                    }
                    vals.Add(val);
                }
            }

            bmp.UnlockBits(bmd);
            return vals;
        }
       

        private AutomationElement CaptureListView(AutomationElement openBatchWindow,ref Bitmap bmp)
        {
            var lstView = openBatchWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, ""));
            bmp = CaptureHelper.CaptureControl((IntPtr)lstView.Current.NativeWindowHandle);
            string sImage = Folders.GetImageFolder() + "batches.jpg";
            Console.WriteLine("snapshot saved at :" + sImage);
            bmp.Save(sImage);
            return lstView;
        }

        private AutomationElement GetOpenBatchesWindow(AutomationElement mainWindow)
        {
            //return mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Open Batches"));

            return mainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Open Batch"));
        }

        private void ClickLastBatch(AutomationElement lstView, int nPosition)
        {
            System.Windows.Rect rect = (System.Windows.Rect)lstView.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            const int eachRowHeight = 19;
            winMessenger.MouseMove((int)rect.Left + 100, (int)rect.Top + nPosition - eachRowHeight/2);
            winMessenger.Click();
        }
        

        private AutomationElement GetOpenBatchesSelectButton(AutomationElement uiElement)
        {
            return uiElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Select"));
        }

     
        private void ClickButton(SystemWindow systemWindow)
        {
            POINT pt = winMessenger.GetWindowCenter(systemWindow);
            winMessenger.MouseMove(pt.X, pt.Y);
            Thread.Sleep(100);
            winMessenger.Click();
        }

    
       
    }
}
