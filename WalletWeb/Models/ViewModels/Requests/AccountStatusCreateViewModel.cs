using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class AccountStatusCreateViewModel
    {
        [Required]
        public int CustomerAccountId { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
