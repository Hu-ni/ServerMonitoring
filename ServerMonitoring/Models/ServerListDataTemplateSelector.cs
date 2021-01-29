using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ServerMonitoring.Models
{
    public class ServerListDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is Task)
            {
                var serveritem = (ServerInfo)item;
                var window = Application.Current.MainWindow;
                if (serveritem.StatusText.Equals("Error"))
                    return
                        window.FindResource("ImportantTaskTemplate") as DataTemplate;
                return
                    window.FindResource("ServerDataTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
