using Microsoft.AspNetCore.Mvc;
using FinalExam.Models.Entities;
using FinalExam.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace FinalExam.Controllers
{
    public class AddUser : Controller
    {
        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AddUser(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            // 檢查資料庫中是否已存在相同的帳號
            var existingUser = await dbManager.User.Where(u => u.UserAccount ==email)
                                 .Select(u => new { u.UserAccount, u.Password })
                                 .FirstOrDefaultAsync();

            // 如果存在，返回 false，否則返回 true
            return existingUser == null;
        }
        [HttpPost]
        public async Task<ActionResult> Register(User user) 
        {
            var NewUser = new User
            {
                UserAccount=user.UserAccount,
                Name = user.Name,
                Password = user.Password,
                //Avatar = null,
                //Points=0,
            };

            if (await IsEmailUnique(user.UserAccount))
            {
                await dbManager.User.AddAsync(NewUser);
                await dbManager.SaveChangesAsync();
                TempData["Message"] = "註冊成功";
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                TempData["ErrorMessage"] = "帳號已存在，請選擇其他帳號！";
                return RedirectToAction("Register","AddUser");
            }
            
            
        }
    }
}
