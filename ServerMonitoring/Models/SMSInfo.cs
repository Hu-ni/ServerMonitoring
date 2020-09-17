using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    class SMSInfo
    {
        private string _id;
        private string _pw;
        private string _to;
        private string _msg;

        public string Id { get => _id; set => _id = value; }
        public string Pw { get => _pw; set => _pw = value; }


    }
}
