using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WalletWeb.Controllers
{
    public class BustedController : Controller
    {
        public ActionResult NotPermitted()
        {
            return View("~/Views/Shared/Error403.cshtml");
        }        
    }
}