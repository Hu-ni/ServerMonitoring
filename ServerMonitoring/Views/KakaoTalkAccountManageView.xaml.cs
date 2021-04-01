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
    /// Interaction logic for KakaoTalkAccountManageView.xaml
    /// </summary>
    public partial class KakaoTalkAccountManageView : Window
    {
        public MainWindow main;
        public KakaoTalkAccountManageView()
        {
            InitializeComponent();
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            main.Sms.Name = tb_kakaotalk_name.Text;
            main.Sms.Id = tb_kakaotalk_id.Text;
            main.Sms.Pw = tb_kakaotalk_pw.Password;
            this.Close();
        }
    }
}
