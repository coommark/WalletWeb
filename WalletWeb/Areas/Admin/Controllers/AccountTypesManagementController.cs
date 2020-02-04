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
    public class AccountTypesManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        int pageSize = 4;
        int totalCount = 0;

        public AccountTypesManagementController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "Id")
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string accountTypesEndPoint = _configuration.GetValue<string>("Api:AccountTypes");
            
            var pagination = Request.Headers["Pagination"];
            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
                int.TryParse(vals[2], out totalCount);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var total = totalCount;
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(accountTypesEndPoint);
                if (response.IsSuccessStatusCode)
                {
                    var resultMessage = response.Content.ReadAsStringAsync().Result;
                    result = resultMessage;
                }
                else if ((int)response.StatusCode == 401)
                {
                    HttpContext.Session.Remove("Token");
                    return RedirectToAction("login", "account", new { area = "" });
                }
            }
            IEnumerable<AccountTypeViewModel> accounts = JsonConvert.DeserializeObject<IEnumerable<AccountTypeViewModel>>(result);
            var model = PagingList.Create(accounts, currentPageSize, page, total, sortExpression, "Id");
            model.RouteValue = new RouteValueDictionary {
                { "filter", filter}
            };
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string accountTypesEndPoint = _configuration.GetValue<string>("Api:AccountTypes");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(accountTypesEndPoint + string.Format("/{0}", id));
                if (response.IsSuccessStatusCode)
                {
                    var resultMessage = response.Content.ReadAsStringAsync().Result;
                    result = resultMessage;
                }
                else if ((int)response.StatusCode == 401)
                {
                    HttpContext.Session.Remove("Token");
                    return RedirectToAction("login", "account", new { area = "" });
                }
            }
           AccountTypeViewModel toReturn = JsonConvert.DeserializeObject<AccountTypeViewModel>(result);
           return View(toReturn);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<SelectListItem> type = new List<SelectListItem>();
            var typeData = new[]{
                     new SelectListItem{ Value="Current Account", Text="Current Account"},
                     new SelectListItem{ Value="Savings Account", Text="Savings Account"},
                 };
            type = typeData.ToList();
            ViewBag.AccountType = type;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountTypeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string accountTypesEndPoint = _configuration.GetValue<string>("Api:AccountTypes");
                
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
                            return RedirectToAction("Index", "AccountTypesManagement");
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
            List<SelectListItem> type = new List<SelectListItem>();
            var typeData = new[]{
                     new SelectListItem{ Value="Current Account", Text="Current Account"},
                     new SelectListItem{ Value="Savings Account", Text="Savings Account"},
                 };
            type = typeData.ToList();
            ViewBag.AccountType = type; return View(model);
        }

        public async Task<JsonResult> Search(string term)
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string searchEndPoint = _configuration.GetValue<string>("Api:AccountTypesSearch");


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(searchEndPoint + string.Format("/{0}", term));
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
            IEnumerable<AccountTypeViewModel> accounts = JsonConvert.DeserializeObject<IEnumerable<AccountTypeViewModel>>(result);
            return Json(accounts.Select(a => new
            {
                label = a.Type,
                val = a.Id
            }));

        }
    }
}