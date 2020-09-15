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
        private ServerXmlFile xml;
        private WebClient client;
        private SmsApi api;

        private string xmlFilePath = @"./server.xml";
        private string sendPhonenumber = "010-4498-2002";
        private string defaultPhonenumber = "010-7470-0449";

        public ServerStatusViewModel()
        {
            xml = new ServerXmlFile(xmlFilePath);
            servers = xml.LoadFile();
            client = new WebClient();
            api = new SmsApi(new SmsApiOptions
            {
                ApiKey = "NCS2KLXFSFWXM0UQ",
                ApiSecret = "B93XWVJNUX57C2HEPILF5OOVHZF170KI",
                DefaultSenderId = defaultPhonenumber
            });
        }

        public ServerInfo SelectServerStatus(int index)
        {
            //IP 주소를 입력
            string address = servers[index].Url;
            if (address.Contains("https://"))
            {
                address = address.Replace("https://", "");
            }
            else if(address.Contains("http://"))
            {
                address = address.Replace("http://", "");
            }
            try
            {
                string htmlText = client.DownloadString("http://" + address + servers[index].DbUrl);
                servers[index].StatusText = "";
                if (servers[index].IsDBAccess)
                {
                    if (htmlText.Contains("DB Success"))
                    {
                        servers[index].StatusText += "Working";
                    }
                    else if (htmlText.Contains("DB Fail"))
                    {
                        servers[index].StatusText += "DB Error";
                        api.SendMessageAsync(sendPhonenumber, servers[index].Name + "서버가" + servers[index].StatusText + "상태입니다! DB 서버를 확인해주세요!");

                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions();

                options.DontFragment = true;

                //전송할 데이터를 입력
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;

                PingReply reply = ping.Send(address, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    servers[index].StatusText += "Apache Error";
                    api.SendMessageAsync(sendPhonenumber, servers[index].Name + "서버가 " +  servers[index].StatusText +"상태입니다! Apache를 확인해주세요!");
                }
                else
                {
                    servers[index].StatusText += "Server Error";
                    api.SendMessageAsync(sendPhonenumber, servers[index].Name + "서버가 " + servers[index].StatusText +"상태입니다! 서버를 재가동 시켜주세요!");
                }
            }

            return servers[index];
        }

        public List<ServerInfo> GetAllServer()
        {
            return servers;
        }

        public void AddServer(ServerInfo server)
        {
            xml.AddServerToXml(server);
            servers.Add(server);
            return;
        }

        public bool DeleteServer(int index)
        {
            if (index >= servers.Count)
                return false;

            xml.RemoveServerToXml(servers[index]);
            servers.RemoveAt(index);
            return true;
        }
    }
}
