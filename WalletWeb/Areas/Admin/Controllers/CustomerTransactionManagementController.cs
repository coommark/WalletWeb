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
    public class CustomerTransactionManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        int pageSize = 4;
        int totalCount = 0;

        public CustomerTransactionManagementController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Transfer()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(CustomerTransactionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string accountTypesEndPoint = _configuration.GetValue<string>("Api:AdminTransfer");

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(apiBaseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                        var json = JsonConvert.SerializeObject(model);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(accountTypesEndPoint, data);

                        if (response.IsSuccessStatusCode)
                        {
                            var resultMessage = response.Content.ReadAsStringAsync().Result;
                            result = resultMessage;
                            return RedirectToAction("Transfer", "CustomerTransactionManagement");
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
                    return View("~/Views/Shared/Error408.cshtml");
                }
            }
            return View(model);
        }        
    }
}