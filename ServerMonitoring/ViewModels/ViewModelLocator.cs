using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ServerMonitoring.Services;

namespace ServerMonitoring.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (ViewModelBase.IsInDesignModeStatic) // 디자인 타임
            {
            }

            //SimpleIoc.Default.ConfigureServices();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        #region ViewModels
        public MainViewModel Main
        {
            get => SimpleIoc.Default.GetInstance<MainViewModel>();
        }
        #endregion

        #region Services
        public KakaoApiService kakaoApiService
        {
            get => SimpleIoc.Default.GetInstance<KakaoApiService>();
        }

        public ServerStatusService serverStatusService
        {
            get => SimpleIoc.Default.GetInstance<ServerStatusService>();
        }
        #endregion
    }
}
