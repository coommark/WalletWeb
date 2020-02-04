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

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = string.Empty;
                string apiBaseUrl = _configuration.GetValue<string>("Api:BaseUrl");
                string regEndPoint = _configuration.GetValue<string>("Api:Register");                
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(apiBaseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var json = JsonConvert.SerializeObject(model);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
     
                        var response = await client.PostAsync(regEndPoint, data);
                        if (response.IsSuccessStatusCode)
                        {
                            var resultMessage = response.Content.ReadAsStringAsync().Result;
                            result = resultMessage;
                            return RedirectToAction("Login", "Account");                            
                        }
                        else if (!response.IsSuccessStatusCode)
                        {
                            
                            var res = response.Content.ReadAsStringAsync().Result;
                            IEnumerable<ApiErrorViewModel> errors = JsonConvert.DeserializeObject<IEnumerable<ApiErrorViewModel>>(res);
                            foreach(var error in errors)
                            {
                                ModelState.AddModelError(string.Empty, error.description);
                            }                            
                            return View(model);
                            

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