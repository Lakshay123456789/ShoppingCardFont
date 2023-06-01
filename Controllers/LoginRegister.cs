using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelfrontEnd.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingCard2FrondEnd.Controllers
{
    public class LoginRegister : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            UserLoginDto userLogin = new UserLoginDto();
            return View(userLogin);
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7036");
                    var response = await client.PostAsJsonAsync<UserLoginDto>("/api/Authentication/login", model);
                    if (response.IsSuccessStatusCode)
                    {
                         var userJson = await response.Content.ReadAsStringAsync();
                         var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResModel>(userJson);
                
                        HttpContext.Session.SetString("Token", deserialized.Token);
                        HttpContext.Session.SetString("Username", deserialized.Username);

                          HttpContext.Session.SetString("UserRole", deserialized.Role);

                        var handler = new JwtSecurityTokenHandler();

                        var jwtToken = handler.ReadJwtToken(deserialized.Token);
                      
                     //   HttpContext.Session.SetString("JwtToken", jwtToken.ToString());
                        // var claims = jwtToken.Claims;
                        // var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
                        var role = roleClaim?.Value;

                       
                        if (!string.IsNullOrEmpty(role))
                        {
                      
                            
                             if (role == "Admin")
                             {
                                return RedirectToAction("Index","Product");
                             }
                           // return RedirectToAction("Privacy", "Home");
                            return RedirectToAction("Index", "Product");
                        }

                    }

                    ModelState.AddModelError("", "SomeThing is Wrong");
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationDto model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var serializedModel = JsonConvert.SerializeObject(model);
                    var stringContent = new StringContent(serializedModel, Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync("https://localhost:7036/api/Authentication/register", stringContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                          
                            return RedirectToAction("Login");
                        }
                        else
                        {
                         
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", errorResponse);
                        }
                    }

                }
            }
            return View(model);

        }
    }
}
