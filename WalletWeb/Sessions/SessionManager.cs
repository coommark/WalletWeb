using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Sessions
{
    public class SessionManager : ISessionManager
    {
        IHttpContextAccessor _contextAccessor;
        public SessionManager(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool InvalidateSession()
        {
            if (_contextAccessor.HttpContext.Session.GetString("Token") != null)
            {
                _contextAccessor.HttpContext.Session.Remove("Token");
                _contextAccessor.HttpContext.Session.Remove("Name");
                _contextAccessor.HttpContext.Session.Remove("Id");
                return true;
            }
            return false;
        }
    }
}
