using Microsoft.AspNetCore.Mvc;
using FinalExam.Models.Entities;
using FinalExam.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace FinalExam.Controllers
{
    public class UserLogin : Controller
    {
        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserLogin(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            Console.WriteLine(model.Email);
            Debug.WriteLine(model.Email);
            var data = await dbManager.User.Where(u => u.UserAccount == model.Email)
                                 //.Select(u => new { u.UserAccount, u.Password })
                                 .FirstOrDefaultAsync();

            //var data2 = await dbManager.Questionnaires
            //                    .FirstOrDefaultAsync();

            //foreach (var prop in data2.GetType().GetProperties())
            //{
            //    var propValue = prop.GetValue(data, null);
            //    Console.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //    Debug.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //}


            //foreach (var prop in data.GetType().GetProperties())
            //{
            //    var propValue = prop.GetValue(data, null);
            //    Console.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //    Debug.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //}
            if (data == null)
            {
                ViewBag.ErrorMessage = "Account not found.";
                return View();
            }

            if (data.Password != model.Password)
            {
                ViewBag.ErrorMessage = "Incorrect password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, data.Name),
                new Claim(ClaimTypes.NameIdentifier, data.UserID.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // 登錄成功
            // 這裡可以設置認證 cookie 或 token 等

            return RedirectToAction("Index", "Home");

        }

        public IActionResult LogOut() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
