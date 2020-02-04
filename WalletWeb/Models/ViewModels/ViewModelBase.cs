using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class ViewModelBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }
}
