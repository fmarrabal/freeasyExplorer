using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;

namespace freeasyExplorer
{
    class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<HWND, uint> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, uint> windows = new Dictionary<HWND, uint>();

            EnumWindows(delegate(HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                uint pid;
                GetWindowThreadProcessId(hWnd, out pid);
                Process[] processes = Process.GetProcessesByName("explorer");

                foreach (var item in processes)
                {
                    if (pid == item.Id)
                    {
                        windows[hWnd] = (uint)item.Id;
                    }
                }
                
                return true;

            }, 0);

            return windows;
        }

        delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern uint
        GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
    
    }
}
