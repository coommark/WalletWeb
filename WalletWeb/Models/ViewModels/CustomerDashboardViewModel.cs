using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public IEnumerable<CustomerAccountViewModel> AllAccounts { get; set; }
        public IEnumerable<CustomerTransactionViewModel> RecentTransactions { get; set; }
    }
}
