using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WalletWeb.Models.ViewModels;

namespace WalletWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string authEndPoint = _configuration.GetValue<string>("Api:Auth");

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", model.Email),
                    new KeyValuePair<string, string>("password", model.Password),
                });
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(apiBaseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.PostAsync(authEndPoint, formContent);
                        if (response.IsSuccessStatusCode)
                        {
                            var resultMessage = response.Content.ReadAsStringAsync().Result;
                            result = resultMessage;
                            JwtViewModel jwt = JsonConvert.DeserializeObject<JwtViewModel>(resultMessage);
                            HttpContext.Session.SetString("Token", jwt.access_token);
                            int statusCode = (int)response.StatusCode;
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(jwt.access_token);
                            var role = token.Claims.First(claim => claim.Type == "role").Value;
                            if (role == "Administrator")
                            {
                                return RedirectToAction("Index", "Home", new { area = "Admin" });
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else if(!response.IsSuccessStatusCode)
                        {
                            if((int)response.StatusCode == 400)
                            {
                                ModelState.AddModelError(string.Empty, "The email or password you entered is incorrect.");
                                return View(model);
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

        [HttpGet]
        public async Task<IActionResult> Accounts()
        {
            var result = string.Empty;

            string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
            string accountTypesEndPoint = _configuration.GetValue<string>("Api:AccountTypes");

            var session = HttpContext.Session.GetString("Token");

            using(var client = new HttpClient())
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
            }
            return Content(result);
        }
    }
}