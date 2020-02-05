using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        public string Type { get; set; }
        public string Body { get; set; }
        public UsersViewModel ApplicationUser { get; set; }
    }
}
