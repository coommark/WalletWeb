using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class CustomerAccountViewModel : ViewModelBase
    {
        public string AccountNumber { get; set; }
        public decimal DailyTransactionLimit { get; set; }
        public decimal Balance { get; set; } = 0.0m;
        public UsersViewModel ApplicationUser { get; set; }
        public AccountTypeViewModel AccountType { get; set; }
        public CustomerAccountStatusViewModel AccountStatus { get; set; }
    }
}
