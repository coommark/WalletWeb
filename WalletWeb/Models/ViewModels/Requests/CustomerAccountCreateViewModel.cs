using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class CustomerAccountCreateViewModel
    {
        [DisplayName("User")]
        public int ApplicationUserId { get; set; }

        [DisplayName("Account Category")]
        public int AccountTypeId { get; set; }

    }
}
