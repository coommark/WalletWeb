using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class CustomerTransactionCreateViewModel
    {
        public int CustomerId { get; set; }
        public string TransactionType { get; set; }
        public decimal Credit { get; set; } = 0.0m;
        public decimal Debit { get; set; } = 0.0m;
        public string Description { get; set; }
        public string Flow { get; set; }
        public int CustomerTransactionBatchId { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }

        //Display Only
        public string AccountNumber { get; set; }
        public string FullName { get; set; }
    }
}
