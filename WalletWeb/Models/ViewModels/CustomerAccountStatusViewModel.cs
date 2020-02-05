using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class CustomerAccountStatusViewModel : ViewModelBase
    {
        public string Status { get; set; }
        public bool IsCurrentStatus { get; set; }
        public string Comment { get; set; }
    }
}
