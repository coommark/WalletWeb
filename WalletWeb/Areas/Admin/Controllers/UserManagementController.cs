using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReflectionIT.Mvc.Paging;
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


        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "Id")
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string endPoint = _configuration.GetValue<string>("Api:Users");
            int size = _configuration.GetValue<int>("Paging:Size");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                PaginationViewModel requestPaging = new PaginationViewModel
                {
                    currentPage = page,
                };

                IEnumerable<string> list = new[]
                {
                    page.ToString(),
                    size.ToString()
                };
                client.DefaultRequestHeaders.Add("Pagination", list);
                var response = await client.GetAsync(endPoint);
                if (response.IsSuccessStatusCode)
                {
                    var pagination = response.Headers.GetValues("Pagination").FirstOrDefault();
                    PaginationViewModel pageModel = JsonConvert.DeserializeObject<PaginationViewModel>(response.Headers.GetValues("Pagination").FirstOrDefault());

                    var resultMessage = response.Content.ReadAsStringAsync().Result;
                    result = resultMessage;
                    IEnumerable<UsersViewModel> users = JsonConvert.DeserializeObject<IEnumerable<UsersViewModel>>(result);
                    var model = PagingList.Create(users, pageModel.itemsPerPage, pageModel.currentPage, pageModel.totalItems, sortExpression, "Id");
                    model.RouteValue = new RouteValueDictionary {
                        { "filter", filter}                    };
                    return View(model);
                }
                else if ((int)response.StatusCode == 401)
                {
                    HttpContext.Session.Remove("Token");
                    return RedirectToAction("login", "account", new { area = "" });
                }
            }
            return null;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string endPoint = _configuration.GetValue<string>("Api:Users");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(endPoint + string.Format("/{0}", id));
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
            UsersViewModel toReturn = JsonConvert.DeserializeObject<UsersViewModel>(result);
            return View(toReturn);
        }

        public async Task<JsonResult> Search(string term)
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string usersEndPoint = _configuration.GetValue<string>("Api:UsersSearch");                       


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