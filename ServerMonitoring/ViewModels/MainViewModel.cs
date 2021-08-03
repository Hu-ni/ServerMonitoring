using GalaSoft.MvvmLight;
using ServerMonitoring.Models;
using ServerMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ServerMonitoring.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ServerStatusService serverService;
        private KakaoApiService kakaoService;

        public MainViewModel(ServerStatusService serverService, KakaoApiService kakaoService)
        {
            this.serverService = serverService ?? throw new ArgumentNullException(nameof(serverService));
            this.kakaoService = kakaoService ?? throw new ArgumentNullException(nameof(kakaoService));
        }

        public void AddServer(ServerInfo server)
        {
            serverService.AddServer(server);
        }

        public bool SetKakaoApi(WebBrowser web)
        {
            if (kakaoService.GetUserToKen(web))
            {
                string accessToken = kakaoService.GetAccessToKen();
                if (!accessToken.Contains("friends"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
