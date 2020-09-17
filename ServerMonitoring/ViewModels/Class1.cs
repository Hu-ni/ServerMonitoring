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

namespace KakaoTalkTest
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);

        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        private static string configPath = Environment.CurrentDirectory + "\\config.xml";

        static void Main(string[] args)
        {
            Console.Title = "KAKAO AUTO LOGIN MADE BY HONSAL (honjasalayo@gmail.com)";
            Console.Clear();

            XElement config = null;
            string id = null;
            string pw = null;
            if (!File.Exists(configPath))
            {
                Console.Write("ID: ");
                id = Console.ReadLine().TrimEnd();

                Console.Write("PW: ");
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        pw += key.KeyChar;
                        Console.Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && pw.Length > 0)
                    {
                        pw = pw.Substring(0, pw.Length - 1);
                        Console.Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine();

                config = new XElement("config",
                    new XElement("id", id),
                    new XElement("pw", pw));

                config.Save(configPath);

                Console.WriteLine($"설정 저장됨: {configPath}");
            }
            Console.WriteLine("설정 파일 읽는 중...");
            config = XElement.Load(configPath);

            if (config != null)
            {
                Console.WriteLine("설정 파일 로드됨");
            }

            id = config.Element("id").Value;
            pw = config.Element("pw").Value;

            Console.WriteLine($"ID: {id}, PW: {pw[0] + "-" + pw[pw.Length - 1]}");

            Console.Write("카카오톡 프로세스 찾는 중...");
            IntPtr H_main_kakao = FindWindow("EVA_Window_Dblclk", null);
            if (IntPtr.Zero == H_main_kakao)
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\KakaoTalk");
                string kakao_path = (string)regKey.GetValue("DisplayIcon", "");
                Process.Start(kakao_path);

                Thread.Sleep(10000);
            }

            Process[] found = Process.GetProcessesByName("kakaotalk");

            while (found == null || found?.Length == 0)
            {
                found = Process.GetProcessesByName("kakaotalk");

                Console.Write(".");
                Thread.Sleep(1500);
            }

            Process kakao = found[0];

            Console.WriteLine();
            Console.WriteLine($"카카오톡 감지! {kakao.ProcessName}({kakao.Id})");

            IntPtr edit1 = FindWindowEx(kakao.MainWindowHandle, IntPtr.Zero, "Edit", null);

            Console.WriteLine("로그인 중...");

            IntPtr edit2 = FindWindowEx(kakao.MainWindowHandle, edit1, "Edit", null);

            SendMessage(edit1, 0xC, IntPtr.Zero, id);
            SendMessage(edit2, 0xC, IntPtr.Zero, pw);

            PostMessage(edit2, 0x100, new IntPtr(0x0D), IntPtr.Zero);
            PostMessage(edit2, 0x101, new IntPtr(0x0D), IntPtr.Zero);

            Thread.Sleep(4000);

            IntPtr msgb = FindWindowEx(kakao.MainWindowHandle, IntPtr.Zero, "#32770", null);
            msgb = FindWindowEx(kakao.MainWindowHandle, msgb, "#32770", null);
            edit1 = FindWindowEx(msgb, IntPtr.Zero, "Edit", null);

            SendMessage(edit1, 0xC, IntPtr.Zero, pw);

            PostMessage(edit1, 0x100, new IntPtr(0x0D), IntPtr.Zero);
            PostMessage(edit1, 0x101, new IntPtr(0x0D), IntPtr.Zero);

            Thread.Sleep(2500);


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
                                SendMessage(hDestEdit, 0x000c, IntPtr.Zero, "박훈");
                                Thread.Sleep(1000);
                                PostMessage(hDestEdit, 0x0100, new IntPtr(0xD), new IntPtr(0x1C001));
                            }
                        }
                    }
                }
            }

            Thread.Sleep(2500);

            IntPtr h_myroom = FindWindow(null, "박훈");
            if (h_myroom != IntPtr.Zero)
            {
                IntPtr h_my_chk = FindWindowEx(h_myroom, IntPtr.Zero, "RICHEDIT50W", "");
                if (h_my_chk != IntPtr.Zero)
                {
                    SendMessage(h_my_chk, 0x000c, IntPtr.Zero, "Test");
                    PostMessage(h_my_chk, 0x0100, new IntPtr(0xD), new IntPtr(0x1C001));
                }
            }

            kakao.Kill();
        }
    }
}
