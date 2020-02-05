using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class CustomerTransactionViewModel : ViewModelBase
    {
        public string TransactionType { get; set; }
        public decimal Credit { get; set; } = 0.0m;
        public decimal Debit { get; set; } = 0.0m;
        public string Description { get; set; }
        public int CustomerTransactionBatchId { get; set; }
        public string AuditDescription { get; set; }
        public string AccountName { get; set; }
        public CustomerAccountViewModel CustomerAccount { get; set; }
    }
}
