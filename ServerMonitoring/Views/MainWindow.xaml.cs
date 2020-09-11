using ServerMonitoring.Models;
using ServerMonitoring.ViewModels;
using ServerMonitoring.Views;
using System;
using System.Collections.Generic;
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

        internal ServerStatusViewModel Server { get => server; private set => server = value; }

        public MainWindow()
        {
            InitializeComponent();
            server = new ServerStatusViewModel();
            RefreshServerList();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            List<ServerInfo> serverList = server.GetAllServer();
            if (serverList.Count == 0)
                return;
            for (int i = 0; i < serverList.Count; i++)
            {
                ServerInfo status = server.SelectServerStatus(i);
                PrintLog("현재 " + status.Url + "의 상태는" + status.StatusText + "입니다.");
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
                items.Add(new ComboBoxItem() { Content = serverList[i].Url });
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
            PrintLog("현재 " + status.Url+ "의 상태는" + status.StatusText + "입니다.");
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
    }
}

