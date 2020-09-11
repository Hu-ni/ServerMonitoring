using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using CoolSms;
using System.Net.NetworkInformation;

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
        private SmsApi api;

        private string txtFilePath = @"./server.txt";
        private string statusUrl = "/statustest.html";
        public ServerStatusViewModel()
        {
            servers = new List<ServerInfo>();
            client = new WebClient();
            api = new SmsApi(new SmsApiOptions
            {
                ApiKey = "NCS2KLXFSFWXM0UQ",
                ApiSecret = "B93XWVJNUX57C2HEPILF5OOVHZF170KI",
                DefaultSenderId = "010-7470-0449"
            });
        }

        public string SelectServerStatus(int index)
        {

            string htmlText = client.DownloadString(servers[index].Url + statusUrl);
            
            if(htmlText.Contains("DB Success"))
            {
                servers[index].StatusText = "DB Success";
            }
            else if(htmlText.Contains("DB Failed"))
            {
                servers[index].StatusText = "DB Failed";
                api.SendMessageAsync("010-7470-0449", "현재" + servers[index].Url + "의 DB 서버 상태에 문제가 발생했습니다!");
            }
            else
            {

                Ping ping = new Ping();
                PingOptions options = new PingOptions();

                options.DontFragment = true;

                //전송할 데이터를 입력
                string data = "aaaaaaaaaaaaaa";
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(data);
                int timeout = 120;

                //IP 주소를 입력
                PingReply reply = ping.Send(servers[index].Url.Replace("http://",""), timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Succeess");
                    servers[index].StatusText = "Apach";

                    api.SendMessageAsync("010-7470-0449", servers[index].Url + "의 아파치 서버가 종료된 상태입니다!");
                }
                else
                {
                    Console.WriteLine("Fail");
                    servers[index].StatusText = "Server Down";

                    api.SendMessageAsync("010-7470-0449", servers[index].Url + "서버가 종료된 상태입니다!");
                }
            }

            return servers[index].StatusText;
        }

        public List<ServerInfo> GetAllServer()
        {
            if (!File.Exists(txtFilePath))
                return servers;

            servers = new List<ServerInfo>();
            string[] serverTxt = File.ReadAllText(txtFilePath).Split('\n');

            for(int i = 0; i < serverTxt.Length; i++)
            {
                servers.Add(new ServerInfo(serverTxt[i]));
            }

            return servers;
        }

        public void AddServer(string url)
        {
            if (File.Exists(txtFilePath))
                File.AppendAllText(txtFilePath, "\n" + url);
            else
                File.WriteAllText(txtFilePath, url);

            servers.Add(new ServerInfo(url));
            return;
        }

        public bool DeleteServer(int index)
        {
            if (!File.Exists(txtFilePath) || index >= servers.Count)
                return false;

            string serverTxt = File.ReadAllText(txtFilePath).Replace(servers[index].Url + "\n", null);

            File.WriteAllText(txtFilePath, serverTxt);
            servers.RemoveAt(index);

            return true;
        }
    }
}
