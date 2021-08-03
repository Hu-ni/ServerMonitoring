using GalaSoft.MvvmLight.Ioc;
using ServerMonitoring.Services;
using ServerMonitoring.ViewModels;
using ServerMonitoring.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ServerMonitoring.Behavior
{
    public class OpenKakaoLoginWindowBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty UrlProperty = DependencyProperty.RegisterAttached(
            "Url",
            typeof(string),
            typeof(OpenKakaoLoginWindowBehavior),
            new FrameworkPropertyMetadata(null));

        public static string GetUrl(DependencyObject d)
        {
            return (string)d.GetValue(UrlProperty);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var service = SimpleIoc.Default.GetInstance<KakaoApiService>();

            KakaoLoginWindow loginWindow = new KakaoLoginWindow(service);

            loginWindow.Show();


        }
    }
}
