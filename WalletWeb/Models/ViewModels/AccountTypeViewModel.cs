using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class AccountTypeViewModel : ViewModelBase
    {
       
        public string Type { get; set; }

        public string Description { get; set; }

        [DisplayName("Category Code")]
        public string CategoryCode { get; set; }

        [DisplayName("Minimum Balance")]
        public decimal MinimumBalance { get; set; }

        [DisplayName("Allow Overdraw?")]
        public bool AllowOverdraw { get; set; }

        public List<CustomerAccountViewModel> CustomerAccounts { get; set; }
    }
}
