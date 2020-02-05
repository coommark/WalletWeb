using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels.Requests
{
    public class MessageCreateViewModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
