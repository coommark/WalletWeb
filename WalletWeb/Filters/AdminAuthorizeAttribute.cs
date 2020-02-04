using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext _context)
        {
            string authToken = _context.HttpContext.Session.GetString("Token");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authToken);
            var role = token.Claims.First(claim => claim.Type == "role").Value;
            if (role == "Administrator")            
                return;            
            else            
                _context.Result = new RedirectToActionResult("NotPermitted", "Busted", new { area = "" });

            base.OnActionExecuting(_context);
        }
    }
}
