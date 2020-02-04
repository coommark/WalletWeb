using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletWeb.Sessions
{
    public interface ISessionManager
    {
        bool InvalidateSession();
    }
}
