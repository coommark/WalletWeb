using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WalletWeb.Filters;
using WalletWeb.Models.ViewModels;
using WalletWeb.Models.ViewModels.Requests;

namespace WalletWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthenticate, AdminAuthorize]
    public class AccountStatusManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        int pageSize = 4;
        int totalCount = 0;

        public AccountStatusManagementController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult SetStatus(int accountId)
        {
            var status = new AccountStatusCreateViewModel
            {
                CustomerAccountId = accountId
            };
            List<SelectListItem> stat = new List<SelectListItem>();
            var statData = new[]{
                     new SelectListItem{ Value="Active", Text="Active"},
                     new SelectListItem{ Value="Dormant", Text="Dormant"},
                     new SelectListItem{ Value="Suspended", Text="Suspended"},
                     new SelectListItem{ Value="Closed", Text="Closed"},
                 };
            stat = statData.ToList();
            ViewBag.Status = stat;
            return View(status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(AccountStatusCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string endPoint = _configuration.GetValue<string>("Api:AccountStatus");
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(apiBaseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");


                        var json = JsonConvert.SerializeObject(model);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(endPoint, data);
                        if (response.IsSuccessStatusCode)
                        {
                            var resultMessage = response.Content.ReadAsStringAsync().Result;
                            result = resultMessage;
                            return RedirectToAction("Details", "CustomerAccountsManagement", new { id = model.CustomerAccountId, area = "Admin" });
                        }
                        else if (!response.IsSuccessStatusCode)
                        {
                            var httpErrorObject = response.Content.ReadAsStringAsync().Result;
                            string res = await response.Content.ReadAsStringAsync();
                            var errors = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                            foreach (var error in errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Value.ToString().Replace("[", "").Replace("]", "").Replace("\"", ""));
                            }
                        };
                    };
                }
                catch (Exception e)
                {
                    return RedirectToAction("Details", "CustomerAccountsManagement", new { id = model.CustomerAccountId, area = "Admin" });
                }
            }
            List<SelectListItem> stat = new List<SelectListItem>();
            var statData = new[]{
                     new SelectListItem{ Value="Active", Text="Active"},
                     new SelectListItem{ Value="Dormant", Text="Dormant"},
                     new SelectListItem{ Value="Suspended", Text="Suspended"},
                     new SelectListItem{ Value="Closed", Text="Closed"},
                 };
            stat = statData.ToList();
            ViewBag.Status = stat;
            return View(model);
        }
    }
}