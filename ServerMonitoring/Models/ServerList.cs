using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    //Biding을 하는데 이걸 오브젝트로 넘기면 안되잖아 Text속성은 스트링이고 그러면 값도 스트링으로 넘겨줘야하는데 Path를 어떻게 넣어야 되지?
    public class ServerList : ObservableCollection<ServerInfo>
    {
        public ServerList() : base()
        {
        }
    }
}
