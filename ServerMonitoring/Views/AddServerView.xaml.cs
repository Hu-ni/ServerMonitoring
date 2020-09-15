using ServerMonitoring.Models;
using ServerMonitoring.ViewModels;
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
using System.Windows.Shapes;

namespace ServerMonitoring.Views
{
    /// <summary>
    /// AddServerView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddServerView : Window
    {
        public MainWindow main;

        private bool _isChecked;

        public AddServerView()
        {
            InitializeComponent();
            _isChecked = false;
            rbtn_no.IsChecked = true;
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo server = new ServerInfo(tb_name.Text, tb_url.Text, _isChecked, tb_dbAccessUrl.Text);
            main.Server.AddServer(server);

            main.RefreshServerList();
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rbtn_yes_Checked(object sender, RoutedEventArgs e)
        {
            _isChecked = true;
        }

        private void rbtn_no_Checked(object sender, RoutedEventArgs e)
        {
            _isChecked = false;
        }
    }
}
