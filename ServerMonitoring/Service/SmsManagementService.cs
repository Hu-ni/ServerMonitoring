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

namespace ServerMonitoring.Services
{
    public class SmsManagementService
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

        private string _name;
        private string _id;
        private string _pw;
        private string _kakaoPath;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        public string Name { get => _name; set => _name = value; }
        public string Id { get => _id; set => _id = value; }
        public string Pw { get => _pw; set => _pw = value; }

        public SmsManagementService()
        {
            Id = "01044982002";
            Pw = "cic2016*";
            Id = "hundl5789@gmail.com";  //테스트용 Id와 비번
            Pw = "asdf123d";
            _kakaoPath = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\KakaoTalk";
        }
        //TODO: 로직 변경
        private bool OpenChatRoom(string title)
        {
            //대화방 열기
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
            return false;
        }

        private void OpenKakao(string title)
        {
            //카톡 열기
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(_kakaoPath);
            string kakao_path = (string)regKey.GetValue("DisplayIcon", "");
            Process.Start(kakao_path);

            Process[] found;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                Thread.Sleep(1000);
                found = Process.GetProcessesByName("kakaotalk");
                Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
                if (stopwatch.Elapsed.TotalSeconds > 10)
                    return;
            }
            while (found == null || found?.Length == 0);
            stopwatch.Stop();
            Process kakao = found[0];

            IntPtr edit1, edit2;
            stopwatch.Start();

            do
            {
                Thread.Sleep(1000);
                edit1 = FindWindowEx(kakao.MainWindowHandle, IntPtr.Zero, "Edit", null);
                edit2 = FindWindowEx(kakao.MainWindowHandle, edit1, "Edit", null);
                if (stopwatch.Elapsed.TotalSeconds > 10)
                    return;
            }
            while (edit1 == IntPtr.Zero || edit2 == IntPtr.Zero);
            stopwatch.Stop();

            //ID, PW 입력
            SendMessage(edit1, 0xC, IntPtr.Zero, Id);
            SendMessage(edit2, 0xC, IntPtr.Zero, Pw);
            //데이터 전달 (Enter 키 값)
            PostMessage(edit2, 0x100, new IntPtr(0x0D), IntPtr.Zero);
            PostMessage(edit2, 0x101, new IntPtr(0x0D), IntPtr.Zero);
        }

        public bool SendSms(string title, string msg)
        {
            bool isSend = false;

            Process[] found = Process.GetProcessesByName("kakaotalk");
            if (found == null || found?.Length == 0)
                OpenKakao(title);

            IntPtr h_myroom = FindWindow(null, title);
            if (h_myroom == IntPtr.Zero)
            {
                OpenChatRoom(title);
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
