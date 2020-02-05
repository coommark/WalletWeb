using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class AccountUpdateRequestViewModel
    {
        public int Id { get; set; }
        public decimal DailyTransactionLimit { get; set; }
    }
}
