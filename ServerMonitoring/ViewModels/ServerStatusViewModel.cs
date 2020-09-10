using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.ViewModels
{
    enum ServerStatus
    {
        DB_SUCESS, DB_FAILED, NO_PAGE
    }
    class ServerStatusViewModel
    {
        private List<ServerInfo> servers;
        private WebClient client;

        private string txtFilePath = @"./server.txt";

        public ServerStatusViewModel()
        {
            servers = new List<ServerInfo>();
            client = new WebClient();
        }

        public string SelectServerStatus(int index)
        {

            string htmlText = client.DownloadString(servers[index].Url);

            if(htmlText.Contains("DB Success"))
            {
                servers[index].StatusText = "DB Success";
            }
            else if(htmlText.Contains("DB Failed"))
            {
                servers[index].StatusText = "DB Failed";
            }
            else
            {
                servers[index].StatusText = "Test";
            }

            return servers[index].StatusText;
        }

        public List<ServerInfo> GetAllServer()
        {
            servers = null;
            if (!File.Exists(@"./server.txt"))
                return servers;

            servers = new List<ServerInfo>();
            string[] serverTxt = File.ReadAllText(@"./server.txt").Split('\n');

            for(int i = 0; i < serverTxt.Length; i++)
            {
                servers.Add(new ServerInfo(serverTxt[i]));
            }

            return servers;
        }

        public void AddServer(string url)
        {
            if (File.Exists(@"./server.txt"))
                File.AppendAllText(@"./server.txt", "\n" + url);
            else
                File.WriteAllText(@"./server.txt", url);

            servers.Add(new ServerInfo(url));
            return;
        }

        public bool DeleteServer(int index)
        {
            if (!File.Exists(@"./server.txt") || index >= servers.Count)
                return false;

            string serverTxt = File.ReadAllText(@"./server.txt").Replace(servers[index].Url + "\n", null);

            File.WriteAllText(txtFilePath, serverTxt);
            servers.RemoveAt(index);

            return true;
        }
    }
}
