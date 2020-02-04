using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int currentPage { get; set; }
        public int itemsPerPage { get; set; }
        public int totalItems { get; set; }
        public int totalPages { get; set; }
    }

}
