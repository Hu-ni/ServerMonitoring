using ServerMonitoring.Constants;
using ServerMonitoring.Models;
using ServerMonitoring.Services;
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
    /// KakaoLoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KakaoLoginWindow : Window
    {
        private KakaoApiService kakaoApi;
        private MainViewModel main;

        public KakaoLoginWindow(KakaoApiService _kakaoApi)
        {
            InitializeComponent();
            kakaoApi = _kakaoApi;
            web_login.Navigate(KakaoKey.KakaoLogInUrl);
        }

        private void Web_login_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (kakaoApi.GetUserToKen(web_login))
            {
                //Console.WriteLine("토큰 얻기 종료");
                string content = kakaoApi.GetAccessToKen();
                Console.WriteLine(content.Contains("friends"));

                if (!content.Contains("friends"))
                {
                    KakaoTokenWindow tokenWindow = new KakaoTokenWindow(kakaoApi);
                    tokenWindow.Top = this.Top + (this.ActualHeight - tokenWindow.ActualHeight) / 2;
                    tokenWindow.Left = this.Left + (this.ActualWidth - tokenWindow.ActualWidth) / 2;
                    tokenWindow.ShowDialog();
                }
                kakaoApi.KakaoLoadFriendList();
                //Console.WriteLine("aaaa: "+ kakaoApi.Data.accessToken);
                this.Close();
            }
            //main.SetKakaoApi();

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
