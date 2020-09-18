using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerMonitoring.ViewModels
{
    class SmsManagementViewModel
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);
        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private string _id;
        private string _pw;
        private string _kakaoPath;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        public SmsManagementViewModel()
        {
            _id = "01044982002";
            _pw = "cic2016*";
            //_id = "hundl5789@gmail.com";  //테스트용 Id와 비번
            //_pw = "asdf123d";
            _kakaoPath = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\KakaoTalk";
        }

        private void OpenKakao(string title)
        {
            IntPtr H_main_kakao = FindWindow("EVA_Window_Dblclk", null);
            if (IntPtr.Zero == H_main_kakao)
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(_kakaoPath);
                string kakao_path = (string)regKey.GetValue("DisplayIcon", "");
                Process.Start(kakao_path);

                Process[] found = Process.GetProcessesByName("kakaotalk");

                while (found == null || found?.Length == 0)
                {
                    found = Process.GetProcessesByName("kakaotalk");
                    Thread.Sleep(1000);
                }

                Process kakao = found[0];

                IntPtr edit1 = FindWindowEx(kakao.MainWindowHandle, IntPtr.Zero, "Edit", null);
                while (edit1 == IntPtr.Zero)
                {
                    edit1 = FindWindowEx(kakao.MainWindowHandle, IntPtr.Zero, "Edit", null);
                    Thread.Sleep(1000);
                }

                IntPtr edit2 = FindWindowEx(kakao.MainWindowHandle, edit1, "Edit", null);
                while (edit2 == IntPtr.Zero)
                {
                    edit2 = FindWindowEx(kakao.MainWindowHandle, edit1, "Edit", null);
                    Thread.Sleep(1000);
                }

                SendMessage(edit1, 0xC, IntPtr.Zero, _id);
                SendMessage(edit2, 0xC, IntPtr.Zero, _pw);

                PostMessage(edit2, 0x100, new IntPtr(0x0D), IntPtr.Zero);
                PostMessage(edit2, 0x101, new IntPtr(0x0D), IntPtr.Zero);

                Thread.Sleep(4000);
            }

            IntPtr hDest = FindWindow("EVA_Window_Dblclk", null); // 메인윈도우 
            if (hDest != IntPtr.Zero)
            {
                hDest = FindWindowEx(hDest, IntPtr.Zero, "EVA_ChildWindow", null); // 메인의 1번 자식 
                if (hDest != IntPtr.Zero)
                {
                    IntPtr hDestEdit = FindWindowEx(hDest, IntPtr.Zero, "EVA_Window", null); // 메인의 1번 자식의 1번 자식 
                    if (hDest != IntPtr.Zero)
                    {
                        hDestEdit = FindWindowEx(hDest, hDestEdit, "EVA_Window", null); //메인의 1번자식의 2번자식 
                        if (hDestEdit != IntPtr.Zero)
                        {
                            hDestEdit = FindWindowEx(hDestEdit, IntPtr.Zero, "Edit", null);
                            //메인의 1번자식의 2번자식의 1번자식 
                            if (hDestEdit != IntPtr.Zero)
                            {
                                SendMessage(hDestEdit, 0x000c, IntPtr.Zero, title);
                                Thread.Sleep(1000);
                                PostMessage(hDestEdit, 0x0100, new IntPtr(0xD), new IntPtr(0x1C001));

                                SendMessage(hDestEdit, 0x000c, IntPtr.Zero, "");
                                Thread.Sleep(1000);
                                PostMessage(hDestEdit, 0x0100, new IntPtr(0xD), new IntPtr(0x1C001));
                            }
                        }
                    }
                }
            }
        }

        public bool SendSms(string title, string msg)
        {
            bool isSend = false;
            IntPtr h_myroom = FindWindow(null, title);
            if (h_myroom == IntPtr.Zero)
            {
                OpenKakao(title);
                h_myroom = FindWindow(null, title);
            }

            IntPtr h_my_chk = FindWindowEx(h_myroom, IntPtr.Zero, "RICHEDIT50W", "");
            if (h_my_chk != IntPtr.Zero)
            {
                SendMessage(h_my_chk, 0x000c, IntPtr.Zero, msg);
                PostMessage(h_my_chk, 0x0100, new IntPtr(0xD), new IntPtr(0x1C001));
                isSend = true;
            }

            return isSend;
        }
    }
}
