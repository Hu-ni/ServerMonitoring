using ServerMonitoring.Models;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using ServerMonitoring.Views;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ServerMonitoring.Services
{
    public enum ServerStatus
    {
        DB_SUCESS, DB_FAILED, NO_PAGE
    }
    //Viewmodel -> Service
    //Service에서는 변수를 특정한 경우 외에는 받아오는 것으로
    public class ServerStatusService
    {
        private List<ServerInfo> servers;
        private ServerXmlFile xml;
        private WebClient client;


        private readonly string xmlFilePath = @"./server.xml";

        public ServerStatusService()
        {
            xml = new ServerXmlFile(xmlFilePath);
            servers = xml.LoadFile();
            client = new WebClient();
            client.Encoding = Encoding.UTF8;

        }

        public async Task<ServerInfo> SelectServerStatus(int index)
        {
            //IP 주소를 입력
            string address = servers[index].Url;
            if (Regex.IsMatch(address, "(?:(ftp|https?|mailto|telnet):\\/\\/)?")) 
            {
                address = Regex.Replace(address, "(?:(ftp|https?|mailto|telnet):\\/\\/)?","");   
                address = "http://" + address;
            }
            try
            {
                //Uri uri = new Uri(address + servers[index].DbUrl);
                //client.DownloadStringTaskAsync(address + servers[index].DbUrl);

                //Async로 변경 address + servers[index].DbUrl
                //PropertyChanged event  
                string htmlText = await client.DownloadStringTaskAsync(address + servers[index].DbUrl);
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
