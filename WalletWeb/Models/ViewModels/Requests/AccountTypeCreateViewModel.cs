using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class AccountTypeCreateViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Description { get; set; }

        [DisplayName("Account Category")]
        public string CategoryCode { get; set; }

        [DisplayName("Minimum Balance")]
        public decimal MinimumBalance { get; set; }

        [DisplayName("Allow Overdraw?")]
        public bool AllowOverdraw { get; set; }
    }
}
