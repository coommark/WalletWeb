using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WalletWeb.Filters;
using WalletWeb.Models;

namespace WalletWeb.Controllers
{
    [SessionAuthenticate]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        public IActionResult Index()
        {
            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            return View();
        }


        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
