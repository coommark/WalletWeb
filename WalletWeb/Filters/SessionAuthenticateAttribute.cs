using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Filters
{
    public class SessionAuthenticateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext _context)
        {
            if (_context.HttpContext.Session.GetString("Token") == null)
            {
                _context.Result = new RedirectToActionResult("login", "account", new { area = "" });
                return;
            }           
            base.OnActionExecuting(_context);
        }       
    }
}
