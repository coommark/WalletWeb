using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WalletWeb.Areas.Admin.Controllers
{
    public class UserManagementController : Controller
    {
        [Area("Admin")]
        public ActionResult Index()
        {
            return View();
        }
        
    }
}