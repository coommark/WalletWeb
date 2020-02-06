using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int AllTransactionsCount { get; set; }
        public int AllAccountsCount { get; set; }
        public int AllAUsersCount { get; set; }
        public int AllMessagesCount { get; set; }

        public IEnumerable<CustomerTransactionViewModel> RecentTransactions { get; set; }
    }
}
