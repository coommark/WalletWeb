using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Models.ViewModels
{
    public class UsersViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
