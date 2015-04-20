using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AppControl;
using freeasyExplorer;
using System.Threading.Tasks;
using HWND = System.IntPtr;

namespace WindowsApplication1
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainWnd : Form
    {
        [DllImport("USER32.DLL")]
        static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        static extern int GetWindowTextLength(HWND hWnd);

        internal delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventProc lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        
#region CONSTANTS
        const uint WINEVENT_OUTOFCONTEXT = 0x0000; // Events are ASYNC
        const uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread
        const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
        const uint WINEVENT_INCONTEXT = 0x0004; // Events are SYNC, this causes your dll to be injected into every process
        const uint EVENT_MIN = 0x00000001;
        const uint EVENT_MAX = 0x7FFFFFFF;
        const uint EVENT_SYSTEM_SOUND = 0x0001;
        const uint EVENT_SYSTEM_ALERT = 0x0002;
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        const uint EVENT_SYSTEM_MENUSTART = 0x0004;
        const uint EVENT_SYSTEM_MENUEND = 0x0005;
        const uint EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;
        const uint EVENT_SYSTEM_MENUPOPUPEND = 0x0007;
        const uint EVENT_SYSTEM_CAPTURESTART = 0x0008;
        const uint EVENT_SYSTEM_CAPTUREEND = 0x0009;
        const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        const uint EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C;
        const uint EVENT_SYSTEM_CONTEXTHELPEND = 0x000D;
        const uint EVENT_SYSTEM_DRAGDROPSTART = 0x000E;
        const uint EVENT_SYSTEM_DRAGDROPEND = 0x000F;
        const uint EVENT_SYSTEM_DIALOGSTART = 0x0010;
        const uint EVENT_SYSTEM_DIALOGEND = 0x0011;
        const uint EVENT_SYSTEM_SCROLLINGSTART = 0x0012;
        const uint EVENT_SYSTEM_SCROLLINGEND = 0x0013;
        const uint EVENT_SYSTEM_SWITCHSTART = 0x0014;
        const uint EVENT_SYSTEM_SWITCHEND = 0x0015;
        const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
        const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
        const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;
        const uint EVENT_SYSTEM_END = 0x00FF;
        const uint EVENT_OEM_DEFINED_START = 0x0101;
        const uint EVENT_OEM_DEFINED_END = 0x01FF;
        const uint EVENT_UIA_EVENTID_START = 0x4E00;
        const uint EVENT_UIA_EVENTID_END = 0x4EFF;
        const uint EVENT_UIA_PROPID_START = 0x7500;
        const uint EVENT_UIA_PROPID_END = 0x75FF;
        const uint EVENT_CONSOLE_CARET = 0x4001;
        const uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;
        const uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;
        const uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;
        const uint EVENT_CONSOLE_LAYOUT = 0x4005;
        const uint EVENT_CONSOLE_START_APPLICATION = 0x4006;
        const uint EVENT_CONSOLE_END_APPLICATION = 0x4007;
        const uint EVENT_CONSOLE_END = 0x40FF;
        const uint EVENT_OBJECT_CREATE = 0x8000; // hwnd ID idChild is created item
        const uint EVENT_OBJECT_DESTROY = 0x8001; // hwnd ID idChild is destroyed item
        const uint EVENT_OBJECT_SHOW = 0x8002; // hwnd ID idChild is shown item
        const uint EVENT_OBJECT_HIDE = 0x8003; // hwnd ID idChild is hidden item
        const uint EVENT_OBJECT_REORDER = 0x8004; // hwnd ID idChild is parent of zordering children
        const uint EVENT_OBJECT_FOCUS = 0x8005; // hwnd ID idChild is focused item
        const uint EVENT_OBJECT_SELECTION = 0x8006; // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        const uint EVENT_OBJECT_SELECTIONADD = 0x8007; // hwnd ID idChild is item added
        const uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008; // hwnd ID idChild is item removed
        const uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009; // hwnd ID idChild is parent of changed selected items
        const uint EVENT_OBJECT_STATECHANGE = 0x800A; // hwnd ID idChild is item w/ state change
        const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B; // hwnd ID idChild is moved/sized item
        const uint EVENT_OBJECT_NAMECHANGE = 0x800C; // hwnd ID idChild is item w/ name change
        const uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D; // hwnd ID idChild is item w/ desc change
        const uint EVENT_OBJECT_VALUECHANGE = 0x800E; // hwnd ID idChild is item w/ value change
        const uint EVENT_OBJECT_PARENTCHANGE = 0x800F; // hwnd ID idChild is item w/ new parent
        const uint EVENT_OBJECT_HELPCHANGE = 0x8010; // hwnd ID idChild is item w/ help change
        const uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011; // hwnd ID idChild is item w/ def action change
        const uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012; // hwnd ID idChild is item w/ keybd accel change
        const uint EVENT_OBJECT_INVOKED = 0x8013; // hwnd ID idChild is item invoked
        const uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014; // hwnd ID idChild is item w? test selection change
        const uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;
        const uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;
        const uint EVENT_OBJECT_END = 0x80FF;
        const uint EVENT_AIA_START = 0xA000;
        const uint EVENT_AIA_END = 0xAFFF;
        
#endregion

        internal enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        } 

        struct UsedWindow
        {
            public IntPtr handle;
            public ApplicationControl control;
            public TabPage tab;
        }

        struct WindowInfo
        {
            public IntPtr handle;
            public uint id;
        }

        private List<uint> registeredHooks = new List<uint>(); 
        private List<WindowInfo> newWindowList = new List<WindowInfo>();
        private List<UsedWindow> usedWindowList = new List<UsedWindow>();
        private TabControl tabControl;
        private TabPage tabPage2;

        public static MainWnd statThis;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MainWnd()
        {
            InitializeComponent();

            FindAllVisibleWindows();
            if (newWindowList.Count <= 0)
            {
                StartNewExplorer();
                FindAllVisibleWindows();
            }

            AddControls();
            statThis = this;

            Task backgroudSearchTask = new Task(new Action(BackgroundExplorerSearch)); 
            backgroudSearchTask.Start();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(988, 673);
            this.tabControl.TabIndex = 0;
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(980, 647);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "+";
            // 
            // MainWnd
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(988, 673);
            this.Controls.Add(this.tabControl);
            this.Name = "MainWnd";
            this.Text = "freeasyExplorer";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainWnd bla = new MainWnd();
            Application.Run(bla);
        }

        private static WinEventProc listener = new WinEventProc(EventCallback);
        delegate void SetTextCallback();

        private void AddControls()
        {
            if(newWindowList.Count <= 0) return;

            while (newWindowList.Count > 0)
            {
                if (this.tabControl.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(AddControls);
                    this.Invoke(d, new object[] { });
                    return;
                }

                UsedWindow tmpWindow = new UsedWindow {handle = IntPtr.Zero ,control = new ApplicationControl(), tab = new TabPage()};

                tmpWindow.control.SuspendLayout();

                tmpWindow.handle = newWindowList[0].handle;
                tmpWindow.control.WindowHandle = newWindowList[0].handle;
                tmpWindow.control.Location = new Point(0, 0);
                tmpWindow.control.Size = new Size(936, 607);
            
                tmpWindow.tab.Controls.Add(tmpWindow.control);
                tmpWindow.tab.BorderStyle = BorderStyle.FixedSingle;


                this.tabControl.TabPages.Add(tmpWindow.tab);
                tmpWindow.control.Dock = DockStyle.Fill;
                
                int length = GetWindowTextLength(tmpWindow.handle);
                if (length != 0)
                {
                    StringBuilder builder = new StringBuilder(length);
                    GetWindowText(tmpWindow.handle, builder, length + 1);
                    tmpWindow.tab.Text = builder.ToString();
                }
                tabControl.SelectedTab = tmpWindow.tab;

                if(registeredHooks.Count <= 0 || !registeredHooks.Contains(newWindowList[0].id))
                {

                    SetWinEventHook(EVENT_MIN, EVENT_MAX, IntPtr.Zero, listener, newWindowList[0].id, 0,
                        WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNTHREAD);
                    registeredHooks.Add(newWindowList[0].id);
                }

                usedWindowList.Add(tmpWindow);
                newWindowList.RemoveAt(0);
            }
        }

        private static void EventCallback(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime)
        {
            //callback function, called when message is intercepted
            Console.WriteLine(iEvent.ToString());
            //if(iEvent == 8 || iEvent == 9)
            //{
                statThis.CustomEvent_Handler();
            //}
        }

        private void CustomEvent_Handler()
        {
            UsedWindow tmpWindow = new UsedWindow { handle = IntPtr.Zero, control = new ApplicationControl(), tab = new TabPage() };

            foreach (var usedWindow in usedWindowList)
            {
                if (usedWindow.tab == tabControl.SelectedTab)
                {
                    tmpWindow = usedWindow;
                }
            }

            int length = GetWindowTextLength(tmpWindow.handle);
            if (length != 0)
            {
                StringBuilder builder = new StringBuilder(length);
                GetWindowText(tmpWindow.handle, builder, length + 1);
                tmpWindow.tab.Text = builder.ToString();
            }
        }

        private void FindAllVisibleWindows()
        {
            foreach (KeyValuePair<IntPtr, uint> window in OpenWindowGetter.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                uint process = window.Value;

                newWindowList.Add(new WindowInfo(){handle = handle, id = process});
            }

            
        }

        private void StartNewExplorer()
        {
            ProcessStartInfo tmp = new ProcessStartInfo();
            tmp.FileName = "explorer.exe";
            tmp.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(tmp);

            Thread.Sleep(1000);
        }

        private void FindOnlyNewWindows()
        {
            foreach (var usedWindow in usedWindowList)
            {
                foreach (var newWindow in newWindowList)
                {
                    if (newWindow.handle == usedWindow.handle)
                    {
                        newWindowList.Remove(newWindow);
                    }
                }
            }
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Text.Equals("+"))
            {
                StartNewExplorer();
                FindAllVisibleWindows();
                FindOnlyNewWindows();
                AddControls();
            }
        }

        private static void BackgroundExplorerSearch()
        {
            while(true)
            {
                Thread.Sleep(500);
                statThis.FindAllVisibleWindows();
                statThis.FindOnlyNewWindows();
                if (statThis.newWindowList.Count > 0)
                {
                    statThis.AddControls();
                }
            }
        }
        
    }
}
