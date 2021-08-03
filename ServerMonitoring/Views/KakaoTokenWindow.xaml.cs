using ServerMonitoring.Constants;
using ServerMonitoring.Services;
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
    /// KakaoTokenWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KakaoTokenWindow : Window
    {
        KakaoApiService kakaoApi;
        public KakaoTokenWindow(KakaoApiService _kakaoApi)
        {
            InitializeComponent();
            kakaoApi = _kakaoApi;
            web_token.Navigate(KakaoKey.kakaoFriendUrl);
        }

        private void web_token_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (kakaoApi.GetUserToKen(web_token))
            {
                //Console.WriteLine("토큰 얻기 종료");
                kakaoApi.GetAccessToKen();
                //Console.WriteLine("accesstoken: " + kakaoApi.Data.accessToken);
                this.Close();
            }
        }


    }
}
