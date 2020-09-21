using ServerMonitoring.Models;
using ServerMonitoring.ViewModels;
using ServerMonitoring.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ServerMonitoring.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerStatusViewModel server;
        private SmsManagementViewModel sms;

        private string _name;

        internal ServerStatusViewModel Server { get => server; private set => server = value; }

        public MainWindow()
        {
            InitializeComponent();
            
            server = new ServerStatusViewModel();
            sms = new SmsManagementViewModel();

            RefreshServerList();

            DispatcherTimer tm_checkServer = new DispatcherTimer();
            tm_checkServer.Interval = TimeSpan.FromMinutes(10);
            tm_checkServer.Tick += new EventHandler(tm_checkServer_Tick);
            tm_checkServer.Start();

            DispatcherTimer tm_saveLog = new DispatcherTimer();
            tm_saveLog.Interval = TimeSpan.FromMinutes(10);
            tm_saveLog.Tick += new EventHandler(tm_saveLog_Tick);
            tm_saveLog.Start();
            Process.GetCurrentProcess().Exited += P_Exited;

            _name = "웹&솔루션 사업팀";
            //_name = "박훈";
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

        private void tm_checkServer_Tick(object sender, EventArgs e)
        {
            List<ServerInfo> serverList = server.GetAllServer();
            if (serverList.Count == 0)
                return;
            for (int i = 0; i < serverList.Count; i++)
            {
                ServerInfo status = server.SelectServerStatus(i);
                string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
                PrintLog(log);

                if (!status.StatusText.Equals("Working"))
                {
                    if (sms.SendSms(_name, log))
                        PrintLog("문자를 성공적으로 전송했습니다.");
                    else
                        PrintLog("문자 전송에 실패했습니다.");
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

        private void btn_serverSelect_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo status = server.SelectServerStatus(cb_serverList.SelectedIndex);
            string log = "현재 " + status.Name + "의 상태는 \"" + status.StatusText + "\"입니다.";
            PrintLog(log);

            if (!status.StatusText.Equals("Working"))
            {
                if (sms.SendSms(_name, log))
                    PrintLog("문자를 성공적으로 전송했습니다.");
                else
                    PrintLog("문자 전송에 실패했습니다.");
            }
        }

        private void cb_serverList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cb_serverList.SelectedItem != null)
                PrintLog(((ComboBoxItem)cb_serverList.SelectedItem).Content + "를 선택했습니다.");
        }

        private void PrintLog(string message)
        {
            tb_actionLog.Text += DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + message + "\n";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Server_log_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".txt", tb_actionLog.Text);
        }

        private void tb_actionLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_actionLog.ScrollToEnd();
        }
    }
}

