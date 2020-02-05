﻿using Microsoft.AspNetCore.Http;
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
                            List<ApiErrorViewModel> errorList = new List<ApiErrorViewModel>();
                            foreach (var error in errors)
                            {
                                var apiError = new ApiErrorViewModel
                                {
                                    description = error.Value.ToString().Replace("[", "").Replace("]", "").Replace("\"", "")
                                };
                                errorList.Add(apiError);
                            }
                            return View("~/Views/Shared/TransactionError.cshtml", errorList);
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

        [HttpGet]
        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "Id")
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string endPoint = _configuration.GetValue<string>("Api:CustomerTransactions");
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
                    IEnumerable<CustomerTransactionViewModel> accounts = JsonConvert.DeserializeObject<IEnumerable<CustomerTransactionViewModel>>(result);
                    var model = PagingList.Create(accounts, pageModel.itemsPerPage, pageModel.currentPage, pageModel.totalItems, sortExpression, "Id");
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
    }
}