using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
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
            SystemWindow[] selectionWindows = SystemWindow.AllToplevelWindows.Where(x => x.Title == winTitle).ToArray();
            if (selectionWindows.Count() == 0)
                return null;
            else
                return selectionWindows[0];
        }
        //public SystemWindow GetStartupWindow()
        //{
        //    return GetWindow("Startup");
        //}
        //public SystemWindow GetShuttingDownWindow()
        //{
        //    return GetWindow("Shutting Down");
        //}
        //public SystemWindow GetSelectionWindow()
        //{
        //    return GetWindow("Selection");
        //}
        //public SystemWindow GetRuntimeControllerWindow()
        //{
        //    return GetWindow("Runtime Controller");
        //}

        //private SystemWindow GetNewRunWindow()
        //{
        //    return GetWindow("New Run");
        //}
        public SystemWindow GetLuminexTopWindow()
        {
            return GetWindow("Luminex100 IS Software");
        }
        private SystemWindow GetNotePadWindow()
        {
            SystemWindow[] selectionWindows = SystemWindow.AllToplevelWindows.Where(x => x.Title.Contains("test.txt")).ToArray();
            return selectionWindows[0];
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


        private void MinimizeWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_SHOWMINNOACTIVE);
        }

        private void ShowWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_SHOW);
        }
        
        
        public void OpenBatches()
        {
            SystemWindow luminexTopWindow = GetLuminexTopWindow();
            IntPtr hwnd = luminexTopWindow.HWnd;
            winMessenger.MaximizeWindow(hwnd);
            //SetForegroundWindow(luminexTopWindow.HWnd);
            AutomationElement uiElement = AutomationElement.FromHandle(hwnd);
            uiElement.SetFocus();
        
            Thread.Sleep(500); 
            try
            {
                winMessenger.Open();
                Thread.Sleep(500);
                winMessenger.ReleaseKey();
                SelectLastBatch(uiElement);
                //winMessenger.MouseMove
                //AutomationElement openBatchSelectButton = GetOpenBatchesSelectButton(uiElement);
                //Click(openBatchSelectButton);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
          
            //PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
            //SendKeys.SendWait("^n");
        }

        private void SelectLastBatch(AutomationElement uiElement)
        {
            var lstView = uiElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, ""));
            System.Windows.Rect rect = (System.Windows.Rect)lstView.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            winMessenger.MouseMove((int)rect.Left + 100, (int)rect.Top + 110);
        }
        

        private AutomationElement GetOpenBatchesSelectButton(AutomationElement uiElement)
        {
            return uiElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Select"));
        }

     

        //internal void WaitForRunWindow()
        //{
        //    log.Info("Wait for run window");
        //    bool bFound = false;
        //    for (int i = 0; i < 20; i++)
        //    {
        //        var runWindow = GetWindow("Runtime Controller");
        //        if (runWindow != null)
        //        {
        //            log.Info("found the window");
        //            bFound = true;
        //            break;
        //        }
        //        Thread.Sleep(250);
        //    }
        //    Thread.Sleep(500);
        //    if (!bFound)
        //        throw new Exception("运行窗口不存在！");

        //}

        //public void ClickRunButton()
        //{
        //    log.Info("try to run script");
        //    var runWindow = GetWindow("Runtime Controller");
        //    if (runWindow == null)
        //        throw new Exception("运行窗口不存在！");
        //    Thread.Sleep(1000);
        //    var newRunWindow = GetNewRunWindow();
        //    if (newRunWindow != null)
        //        ClickNewButton(newRunWindow);
        //    log.Info("click run button");
        //    ClickRunButton(runWindow);
        //    log.Info("minimize");
        //    MinimizeWindow(runWindow.HWnd);
        //}


        //private void ClickNewButton(SystemWindow runWindow)
        //{
        //    var newButton = GetNewRunWindow();
        //    if (newButton == null)
        //        throw new Exception("新建按钮无法找到！");
        //    ClickButton(newButton);
        //}

        //private void ClickRunButton(SystemWindow runWindow)
        //{
        //    int retryTimes = 6;
        //    for (; ; )
        //    {
        //        SystemWindow[] runButtons = runWindow.AllDescendantWindows.Where(x => x.Title == "Run" && x.Visible).ToArray();
        //        if (runButtons.Count() == 0)
        //        {
        //            retryTimes--;
        //            Thread.Sleep(400);
        //            if (retryTimes == 0)
        //                throw new Exception("运行按钮无法找到！");
        //        }
        //        else
        //        {
        //            ClickButton(runButtons.First());
        //            break;
        //        }
        //    }
        //}

        private void ClickButton(SystemWindow systemWindow)
        {
            POINT pt = winMessenger.GetWindowCenter(systemWindow);
            winMessenger.MouseMove(pt.X, pt.Y);
            Thread.Sleep(100);
            winMessenger.Click();
        }

        private AutomationElement FindButton(AutomationElement currentWindow, string buttonName)
        {
            var isButtonCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            var isEnableCondition = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            var isOkCondition = new PropertyCondition(AutomationElement.NameProperty, buttonName);
            var enabledButtonCondition = new AndCondition(new Condition[] { isButtonCondition, isEnableCondition, isOkCondition });
            var okButton = currentWindow.FindFirst(TreeScope.Descendants, enabledButtonCondition);
            if (okButton == null)
                throw new Exception("无法找到下一步按钮！");
            return okButton;
        }

        internal void HideWindow(SystemWindow window)
        {
            ShowWindow(window.HWnd, SW_HIDE);
        }
    }
}
