using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    public class TimeoutWebClient : WebClient
    {
        int timeout = 0;

        public TimeoutWebClient(int _timeout)
        {
            timeout = _timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = timeout;

            return request;
        }
    }
}
