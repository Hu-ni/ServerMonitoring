using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    class ServerInfo
    {
        private string url;
        private string statusText;

        public string Url { get => url; private set => url = value; }
        public string StatusText { get => statusText; set => statusText = value; }

        public ServerInfo(string url, string statusText = null)
        {
            this.url = url ?? throw new ArgumentNullException(nameof(url));
            this.statusText = statusText;
        }
    }
}
