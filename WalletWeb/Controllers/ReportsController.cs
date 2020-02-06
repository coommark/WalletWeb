using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WalletWeb.Filters;
using WalletWeb.Models.ViewModels;
using WalletWeb.Models.ViewModels.Requests;


namespace WalletWeb.Controllers
{
    [SessionAuthenticate]
    public class ReportsController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> AccountStatement()
        {
            var result = string.Empty;
            List<CustomerAccountViewModel> accounts = new List<CustomerAccountViewModel>();
            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string endPoint = _configuration.GetValue<string>("Api:AllAccountsForCustomer");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = await client.GetAsync(endPoint);
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
                accounts = JsonConvert.DeserializeObject<IEnumerable<CustomerAccountViewModel>>(result).ToList();
            }
            if (accounts.Count > 0)
            {
                var savings = accounts.SingleOrDefault(x => x.AccountType.Type == "Savings Account");
                var current = accounts.SingleOrDefault(x => x.AccountType.Type == "Current Account");
                if (savings != null)
                {
                    ViewBag.Savings = savings;
                }

                if (current != null)
                {
                    ViewBag.Current = current;
                }

                List<SelectListItem> accountsList = accounts.Select(x => new SelectListItem { Text = x.AccountType.Type + " - " + x.AccountNumber, Value = x.AccountNumber }).ToList();
                ViewBag.Accounts = accountsList;
            }
            StatementRequestViewModel model = new StatementRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now
            };            

            List<SelectListItem> formatlist = new List<SelectListItem>();
            var formatdata = new[]{
                 new SelectListItem{ Value="PDF",Text="PDF"},
                 new SelectListItem{ Value="Excel",Text="Excel"},
             };
            formatlist = formatdata.ToList();
            ViewBag.Format = formatlist;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountStatement(StatementRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string endPoint = _configuration.GetValue<string>("Api:AccountStatement");
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

                            if (model.Format == "PDF")
                            {
                                IEnumerable<CustomerTransactionViewModel> output = JsonConvert.DeserializeObject<IEnumerable<CustomerTransactionViewModel>>(result);
                                return new ViewAsPdf("AccountStatementPdf", output)
                                {
                                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                                    PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                                    CustomSwitches = "--header-spacing \"10\" " +
                                      "--footer-center \"  Date: " +
                                      DateTime.Now.Date.ToString("dd/MM/yyyy") + "  Page: [page] of [toPage]\"" +
                                      " --footer-font-size \"10\" --footer-spacing 1"
                                };
                            }
                            else
                            {
                                IEnumerable<TransactionsReportViewModel> output = JsonConvert.DeserializeObject<IEnumerable<TransactionsReportViewModel>>(result);
                                var stream = new MemoryStream();

                                using (var package = new ExcelPackage(stream))
                                {
                                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                                    workSheet.Cells.LoadFromCollection(output, true);
                                    package.Save();
                                }
                                stream.Position = 0;
                                string excelName = $"AccountStatement-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                                return File(stream, "application/ms-excel", excelName);
                            }

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
                    return null;
                }
            }
            List<SelectListItem> typeList = new List<SelectListItem>();
            var typeData = new[]{
                     new SelectListItem{ Value="All", Text="All"},
                     new SelectListItem{ Value="Debits", Text="Debits"},
                     new SelectListItem{ Value="Credits", Text="Credits"},
                 };
            typeList = typeData.ToList();
            ViewBag.Type = typeList;

            List<SelectListItem> formatlist = new List<SelectListItem>();
            var formatdata = new[]{
                 new SelectListItem{ Value="PDF",Text="PDF"},
                 new SelectListItem{ Value="Excel",Text="Excel"},
             };
            formatlist = formatdata.ToList();
            ViewBag.Format = formatlist;
            return View(model);
        }
    }
}