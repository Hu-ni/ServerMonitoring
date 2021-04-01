using GalaSoft.MvvmLight;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerMonitoring.Models;
using ServerMonitoring.Services;
using ServerMonitoring.ViewModels;
using ServerMonitoring.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace ServerMonitoring.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModelBase ViewModel
        {
            get => DataContext as ViewModelBase;
            set => DataContext = value;
        }

        // 시스템 트레이 추가
        // UI 직관적으로 변경
        // 카카오톡 설정 추가
        // 카카오톡 쓰레드로 변경 1
        private ServerStatusService server;
        private SmsManagementService sms;
        private NotifyIcon ni;
        private string _name;

        public ServerStatusService Server { get => server; private set => server = value; }
        public SmsManagementService Sms { get => sms; private set => sms = value; }

        public KakaoApiService kakaoApi { get; set; } = new KakaoApiService(new KakaoData());

        public MainWindow()
        {
            InitializeComponent();
            
            server = new ServerStatusService();
            Sms = new SmsManagementService();
            ni = new NotifyIcon();

            RefreshServerList();

            DispatcherTimer tm_checkServer = new DispatcherTimer();
            tm_checkServer.Interval = TimeSpan.FromMinutes(10);
            tm_checkServer.Tick += new EventHandler(tm_checkServer_Tick);
            tm_checkServer.Start();

            DispatcherTimer tm_saveLog = new DispatcherTimer();
            tm_saveLog.Interval = TimeSpan.FromMinutes(60);
            tm_saveLog.Tick += new EventHandler(tm_saveLog_Tick);
            tm_saveLog.Start();
            Process.GetCurrentProcess().Exited += P_Exited;

            _name = "웹&솔루션 사업팀";
            _name = "박훈";

            ContextMenu menu = new ContextMenu();
            MenuItem item1 = new MenuItem();
            item1.Index = 0;
            item1.Text = "열기";    // menu 이름

            item1.Click += delegate (object click, EventArgs eClick)    // menu 의 클릭 이벤트 등록
            {
                Method1();
            };
            MenuItem item2 = new MenuItem();    // menu 객체에 들어갈 각 menu
            item2.Index = 1;
            item2.Text = "닫기";    // menu 이름
            item2.Click += delegate (object click, EventArgs eClick)    // menu의 클릭 이벤트 등록
            {
                Method2();
            };

            menu.MenuItems.Add(item1);    // Menu 객체에 각각의 menu 등록
            menu.MenuItems.Add(item2);    // Menu 객체에 각각의 menu 등록

            ni.Icon = Properties.Resources.monitoring;    // 아이콘 등록 1번째 방법
            ni.Visible = true;
            ni.DoubleClick += delegate (object senders, EventArgs args)    // Tray icon의 더블 클릭 이벤트 등록
            {
                DoubleMethod();
            };
            ni.ContextMenu = menu;    // Menu 객체 등록
            ni.Text = "서버 모니터링 프로그램";    // Tray icon 이름
        }
        public void Method1() 
        {
            this.Visibility = Visibility.Visible;
        }
        public void Method2() 
        {
            Process.GetCurrentProcess().Close();
        }
        public void DoubleMethod() 
        {
            this.Visibility = Visibility.Visible;
        }


        private void tm_saveLog_Tick(object sender, EventArgs e)
        {
            if(!Directory.Exists(@"./" + DateTime.Now.ToString("yyyyMMdd")))
                Directory.CreateDirectory(@"./" + DateTime.Now.ToString("yyyyMMdd"));
            File.WriteAllText(@"./" + DateTime.Now.ToString("yyyyMMdd") + "/Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
            tb_actionLog.Clear();
        }

        private void P_Exited(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"./" + DateTime.Now.ToString("yyyyMMdd")))
                Directory.CreateDirectory(@"./" + DateTime.Now.ToString("yyyyMMdd"));
            File.WriteAllText(@"./"+DateTime.Now.ToString("yyyyMMdd")+"/Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
        }

        private async void tm_checkServer_Tick(object sender, EventArgs e)
        {
            List<ServerInfo> serverList = server.GetAllServer();
            if (serverList.Count == 0)
                return;
            for (int i = 0; i < serverList.Count; i++)
            {
                ServerInfo status = await server.SelectServerStatus(i);
                string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
                await PrintLog(log);
                if (!status.StatusText.Equals("Working"))
                {
                    JObject SendJson = new JObject();
                    JObject LinkJson = new JObject();

                    LinkJson.Add("web_url", status.Url);
                    LinkJson.Add("mobile_web_url", status.Url);

                    SendJson.Add("object_type", "text");
                    SendJson.Add("text", log);
                    SendJson.Add("link", LinkJson);
                    SendJson.Add("button_title", "사이트 바로가기");

                    IRestResponse response = kakaoApi.KakaoDefaultSendMessage(SendJson);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        await PrintLog("문자를 성공적으로 전송했습니다.");
                    else
                        await PrintLog("문자 전송에 실패했습니다.");
                }
            }
        }

        public void RefreshServerList()
        {
            List<ComboBoxItem> items = new List<ComboBoxItem>();

            List<ServerInfo> serverList = server.GetAllServer();
            if (serverList == null)
                return;
            for (int i = 0; i < serverList.Count; i++)
            {
                items.Add(new ComboBoxItem() { Content = serverList[i].Name });
            }
            cb_serverList.ItemsSource = items;
        }

        private void btn_addServer_Click(object sender, RoutedEventArgs e)
        {
            AddServerView addServerView = new AddServerView();
            addServerView.main = this;
            addServerView.Show();
        }

        private void btn_deleteServer_Click(object sender, RoutedEventArgs e)
        {
            server.DeleteServer(cb_serverList.SelectedIndex);
            RefreshServerList();
        }

        private async void btn_serverSelect_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo status = await server.SelectServerStatus(cb_serverList.SelectedIndex);
            string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
            await PrintLog(log);

            JObject SendJson = new JObject();
            JObject LinkJson = new JObject();

            LinkJson.Add("web_url", status.Url);
            LinkJson.Add("mobile_web_url", status.Url);

            SendJson.Add("object_type", "text");
            SendJson.Add("text", log);
            SendJson.Add("link", LinkJson);
            SendJson.Add("button_title", "사이트 바로가기");

            IRestResponse response = kakaoApi.KakaoDefaultSendMessage(SendJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                await PrintLog("문자를 성공적으로 전송했습니다.");
            else
                await PrintLog("문자 전송에 실패했습니다.");
        }

        private void cb_serverList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cb_serverList.SelectedItem != null)
                PrintLog(((ComboBoxItem)cb_serverList.SelectedItem).Content + "를 선택했습니다.");
        }

        private Task PrintLog(string message)
        {
            tb_actionLog.Text += DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + message + "\n";
            return Task.CompletedTask;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
        }

        private void tb_actionLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_actionLog.ScrollToEnd();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("프로그램을 종료하시겠습니까?", "YesOrNo", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                ni.Visible = true;
                this.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        private void btn_acManager_Click(object sender, RoutedEventArgs e)
        {
            KakaoLoginWindow accountManageView = new KakaoLoginWindow(kakaoApi);
            accountManageView.Show();
        }

        private void btn_acManager_Click2(object sender, RoutedEventArgs e)
        {
            kakaoApi.KakaoTalkLogOut();
        }

        private void WebBrowserVersionSetting()
        {
            RegistryKey registryKey = null; // 레지스트리 변경에 사용 될 변수

            int browserver = 0;
            int ie_emulation = 0;
            var targetApplication = Process.GetCurrentProcess().ProcessName + ".exe"; // 현재 프로그램 이름

            // 사용자 IE 버전 확인
            using (System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser())
            {
                browserver = wb.Version.Major;
                if (browserver >= 11)
                    ie_emulation = 11001;
                else if (browserver == 10)
                    ie_emulation = 10001;
                else if (browserver == 9)
                    ie_emulation = 9999;
                else if (browserver == 8)
                    ie_emulation = 8888;
                else
                    ie_emulation = 7000;
            }

            try
            {
                registryKey = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);

                // IE가 없으면 실행 불가능
                if (registryKey == null)
                {
                    MessageBox.Show("웹 브라우저 버전 초기화에 실패했습니다..!");
                    System.Windows.Forms.Application.Exit();
                    return;
                }

                string FindAppkey = Convert.ToString(registryKey.GetValue(targetApplication));

                // 이미 키가 있다면 종료
                if (FindAppkey == ie_emulation.ToString())
                {
                    registryKey.Close();
                    return;
                }

                // 키가 없으므로 키 셋팅
                registryKey.SetValue(targetApplication, unchecked((int)ie_emulation), RegistryValueKind.DWord);

                // 다시 키를 받아와서
                FindAppkey = Convert.ToString(registryKey.GetValue(targetApplication));

                // 현재 브라우저 버전이랑 동일 한지 판단
                if (FindAppkey == ie_emulation.ToString())
                {
                    return;
                }
                else
                {
                    MessageBox.Show("웹 브라우저 버전 초기화에 실패했습니다..!");
                    System.Windows.Forms.Application.Exit();
                    return;
                }
            }
            catch
            {
                MessageBox.Show("웹 브라우저 버전 초기화에 실패했습니다..!");
                System.Windows.Forms.Application.Exit();
                return;
            }
            finally
            {
                // 키 메모리 해제
                if (registryKey != null)
                {
                    registryKey.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WebBrowserVersionSetting();
        }

        public void Send()
        {

        }

        private void btn_loadFriend_Click(object sender, RoutedEventArgs e)
        {
            kakaoApi.KakaoLoadFriendList();
        }
    }
}

