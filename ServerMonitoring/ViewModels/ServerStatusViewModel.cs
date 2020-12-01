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
    public enum ServerStatus
    {
        DB_SUCESS, DB_FAILED, NO_PAGE
    }
    public class ServerStatusViewModel
    {
        private List<ServerInfo> servers;
        private ServerXmlFile xml;
        private WebClient client;

        private readonly string xmlFilePath = @"./server.xml";

        public ServerStatusViewModel()
        {
            xml = new ServerXmlFile(xmlFilePath);
            servers = xml.LoadFile();
            client = new WebClient();
        }

        public ServerInfo SelectServerStatus(int index)
        {
            //IP 주소를 입력
            string address = servers[index].Url;
            if (address.Contains("https://"))
                address = address.Replace("https://", "");
            else if(address.Contains("http://"))
                address = address.Replace("http://", "");
            
            try
            {
                string htmlText = client.DownloadString("http://" + address + servers[index].DbUrl);
                servers[index].StatusText = "";
                if (servers[index].IsDBAccess)
                {
                    if (htmlText.Contains("DB Success"))
                        servers[index].StatusText += "Working";
                    else if (htmlText.Contains("DB Fail") || htmlText.Contains("mysql_connect error"))
                        servers[index].StatusText += "DB Error";
                    else
                        return PingTest(address, index);
                }
            }
            catch (WebException e)
            {
                servers[index].StatusText = e.Message;
            }

            return servers[index];
        }

        private ServerInfo PingTest(string address, int index)
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
                servers[index].StatusText += "Apache Error";
            else
                servers[index].StatusText += "Server Error";
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
