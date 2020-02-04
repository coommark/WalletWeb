using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WalletWeb.Filters;
using WalletWeb.Models.ViewModels;

namespace WalletWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthenticate, AdminAuthorize]
    public class UserManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        int pageSize = 4;
        int totalCount = 0;

        public UserManagementController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Area("Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> Search(string term)
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string usersEndPoint = _configuration.GetValue<string>("Api:Users");                       


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(usersEndPoint + string.Format("/{0}", term));
                if (response.IsSuccessStatusCode)
                {
                    var resultMessage = response.Content.ReadAsStringAsync().Result;
                    result = resultMessage;
                }
                else if ((int)response.StatusCode == 401)
                {
                    HttpContext.Session.Remove("Token");
                    return null;
                }
            }
            IEnumerable<UsersViewModel> users = JsonConvert.DeserializeObject<IEnumerable<UsersViewModel>>(result);
            return Json(users.Select(a => new
            {
                label = a.FullName,
                val = a.Id
            }));

        }

    }
}