﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    public class ServerInfo : INotifyPropertyChanged
    {
        private string _name;
        private string _url;
        private bool _isDBAccess;
        private string _dbUrl;
        private string _statusText;

        public string Name { get => _name; private set => _name = value; }
        public string Url { get => _url; private set => _url = value; }
        public bool IsDBAccess { get => _isDBAccess; private set => _isDBAccess = value; }
        public string DbUrl { get => _dbUrl; private set => _dbUrl = value; }
        public string StatusText { get => _statusText; set => _statusText = value; }

        public ServerInfo(string name, string url, bool isDBAccess, string dbUrl)
        {
            if (name.Equals(""))
                Name = url;
            else
                Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            IsDBAccess = isDBAccess;
            DbUrl = dbUrl ?? throw new ArgumentNullException(nameof(dbUrl));
            StatusText = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString() => _name;

        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
