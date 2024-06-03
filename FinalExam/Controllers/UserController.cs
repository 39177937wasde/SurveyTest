using FinalExam.Data;
using FinalExam.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace FinalExam.Controllers
{
    public class UserController : Controller
    {
        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ChangeProfileAsync()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Debug.WriteLine(userName);

            var user = await dbManager.User.Where(u => u.UserID == long.Parse(userName))
                                           .FirstOrDefaultAsync();

            var model = new ChangeProfileModel
            {
                Name = user.Name,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfile(ChangeProfileModel model)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Debug.WriteLine(userName);
            var user = await dbManager.User.Where(u => u.UserID == long.Parse(userName))
                                           .FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {

                if (user != null)
                {
                    if (string.IsNullOrEmpty(model.ChangeName) && string.IsNullOrEmpty(model.ChangePassword))
                    {
                        goto noinput;
                    }
                    if (!string.IsNullOrEmpty(model.ChangeName))
                    {
                        user.Name = model.ChangeName;
                    }

                    if (!string.IsNullOrEmpty(model.ChangePassword))
                    {
                        user.Password = model.ChangePassword;
                    }

                    await dbManager.SaveChangesAsync();
                    TempData["Message"] = "資料修改成功";
                   
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, "User")
                };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    //return RedirectToAction(nameof(ChangeProfile));
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"模型驗證錯誤：{key} - {error.ErrorMessage}");
                    }
                }
            }

        noinput: TempData["Message"] = "請至少輸入一個欄位！";
            model.Name = user.Name;
            return View(model);
        }
    }
}
