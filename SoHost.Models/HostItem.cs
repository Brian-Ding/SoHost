using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoHost.Models
{
    public class HostItem : BindableBase
    {
        private string _ip;
        public string IP { get => _ip; set => SetProperty(ref _ip, value); }

        private string _url;
        public string URL { get => _url; set => SetProperty(ref _url, value); }

        private bool _isChecked = true;
        public bool IsChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }

        private string _remark;
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }
    }
}
