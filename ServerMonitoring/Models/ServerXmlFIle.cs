using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ServerMonitoring.Models
{
    class ServerXmlFile
    {
        private XDocument _doc;
        private XElement _root;
        private string _filePath;

        public ServerXmlFile(string filePath)
        {
            _doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            _filePath = filePath;
        }

        public XDocument Doc { get => _doc; private set => _doc = value; }
        public XElement Root { get => _root; private set => _root = value; }
        public string FilePath { get => _filePath; private set => _filePath = value; }

        public void CreateFile()
        {
            _root = new XElement("servers");
            _doc.Add(_root);

            _doc.Save(_filePath);
        }

        public List<ServerInfo> LoadFile()
        {
            List<ServerInfo> servers = new List<ServerInfo>();
            if (!File.Exists(_filePath))
                CreateFile();

            _doc = XDocument.Load(_filePath);
            _root = _doc.Element("servers");

            IEnumerable<XElement> elems = _root.Elements("server");
            foreach (XElement elem in elems)
            {
                ServerInfo server = new ServerInfo(elem.Element("name").Value,
                                    elem.Element("url").Value,
                                    elem.Element("db_access").Value.ToLower().Equals("true"),
                                    elem.Element("db_access_url").Value);
                servers.Add(server);
            }
            return servers;
        }

        public void AddServerToXml(ServerInfo server)
        { 
            XElement elem = new XElement("server",
                new XElement("name", server.Name),
                new XElement("url", server.Url),
                new XElement("db_access", server.IsDBAccess.ToString()),
                new XElement("db_access_url", server.DbUrl));

            _root.Add(elem);
            _doc.Save(_filePath);
        }

        public void RemoveServerToXml(ServerInfo server)
        {
            _root.Elements("server").Where(p => p.Element("name").Value == server.Name).Remove();
            _doc.Save(_filePath);
        }
    }
}
