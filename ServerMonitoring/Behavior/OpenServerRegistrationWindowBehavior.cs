using GalaSoft.MvvmLight.Ioc;
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
    public class OpenServerRegistrationWindowBehavior : Behavior<Button>
    {
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
            var viewModel = SimpleIoc.Default.GetInstance<MainViewModel>();

            AddServerWindow addServer = new AddServerWindow(viewModel);

            addServer.Show();
        }
    }
}
