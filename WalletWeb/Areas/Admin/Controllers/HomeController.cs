using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletWeb.Filters;

namespace WalletWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthenticate, AdminAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}