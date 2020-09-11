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

        public AddServerView()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            main.Server.AddServer(tb_url.Text.Trim());
            main.RefreshServerList();
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
