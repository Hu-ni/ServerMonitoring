using ServerMonitoring.Models;
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
        // 시스템 트레이 추가
        // UI 직관적으로 변경
        // 카카오톡 설정 추가
        // 카카오톡 쓰레드로 변경 1
        private ServerStatusViewModel server;
        private SmsManagementViewModel sms;
        private NotifyIcon ni;
        private string _name;

        public ServerStatusViewModel Server { get => server; private set => server = value; }
        
        public MainWindow()
        {
            InitializeComponent();
            
            server = new ServerStatusViewModel();
            sms = new SmsManagementViewModel();
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

            ni.Icon = new System.Drawing.Icon("setting.ico");    // 아이콘 등록 1번째 방법
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
            MessageBox.Show("Method1");
        }
        public void Method2() 
        {
            MessageBox.Show("Method2");
            Process.GetCurrentProcess().Close();
        }
        public void DoubleMethod() 
        {
            this.Visibility = Visibility.Visible;
        }


        private void tm_saveLog_Tick(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
            tb_actionLog.Clear();
        }

        private void P_Exited(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
        }

        private async void tm_checkServer_Tick(object sender, EventArgs e)
        {
            List<ServerInfo> serverList = server.GetAllServer();
            if (serverList.Count == 0)
                return;
            for (int i = 0; i < serverList.Count; i++)
            {
                ServerInfo status = server.SelectServerStatus(i);
                string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
                await PrintLog(log);
                if (!status.StatusText.Equals("Working"))
                {
                    if (sms.SendSms(_name, log))
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
            ServerInfo status = server.SelectServerStatus(cb_serverList.SelectedIndex);
            string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
            await PrintLog(log);

            //if (!status.StatusText.Equals("Working"))
            //{
                if (sms.SendSms(_name, log))
                    await PrintLog("문자를 성공적으로 전송했습니다.");
                else
                    await PrintLog("문자 전송에 실패했습니다.");
            //}
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
            ni.Visible = true;
            this.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}

