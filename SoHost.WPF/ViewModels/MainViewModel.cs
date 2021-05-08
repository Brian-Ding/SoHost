using SoHost.Models;
using SoHost.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SoHost.WPF.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _ip;
        public string IP { get => _ip; set => SetProperty(ref _ip, value); }

        private string _url;
        public string URL { get => _url; set => SetProperty(ref _url, value); }

        private string _remark;
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        private bool _checkAll;
        public bool CheckAll
        {
            get => _checkAll;
            set
            {
                if (SetProperty(ref _checkAll, value))
                {
                    foreach (var item in Hosts)
                    {
                        item.IsChecked = CheckAll;
                    }
                }
            }
        }

        private string _message;
        public string Message { get => _message; set => SetProperty(ref _message, $"{DateTime.Now: yyyy-MM-dd HH:mm:ss}\t{value}"); }

        public ObservableCollection<HostItem> Hosts { get; } = new ObservableCollection<HostItem>();

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }

        public MainViewModel()
        {
            ApplyCommand = new DelegateCommand(Apply);
            AddCommand = new DelegateCommand(Add, CanAdd)
                .ObservesProperty(() => IP)
                .ObservesProperty(() => URL);
            UpdateCommand = new DelegateCommand<HostItem>(Update);
            RemoveCommand = new DelegateCommand<HostItem>(Remove);

            if (File.Exists("config.json"))
            {
                var hosts = JsonSerializer.Deserialize<HostItem[]>(File.ReadAllText("config.json"));
                foreach (var item in hosts)
                {
                    Hosts.Add(item);
                }
            }
        }

        private void Update(HostItem obj)
        {
            IP = obj.IP;
            URL = obj.URL;
            Remark = obj.Remark;
        }

        private void Remove(HostItem obj)
        {
            Hosts.Remove(obj);
        }

        private bool CanAdd()
        {
            return !string.IsNullOrEmpty(IP) && !string.IsNullOrEmpty(URL);
        }

        private void Add()
        {
            HostItem host = new HostItem() { IP = IP, URL = URL, Remark = Remark };
            Hosts.Add(host);
        }

        private void Apply()
        {
            string json = JsonSerializer.Serialize(Hosts.ToArray());
            File.WriteAllText("config.json", json);

            StringBuilder sb = new StringBuilder();
            foreach (var item in Hosts)
            {
                if (!item.IsChecked)
                {
                    sb.Append("# ");
                }
                sb.Append($"{item.IP} {item.URL}");
                if (!string.IsNullOrEmpty(item.Remark))
                {
                    sb.Append($" #{item.Remark}");
                }
                sb.Append("\n");
            }
            ModifyHostsFile(sb.ToString());
        }

        public bool ModifyHostsFile(string entry)
        {
            try
            {
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts"), entry);

                Message = "Applied successfully!";

                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }
    }
}
