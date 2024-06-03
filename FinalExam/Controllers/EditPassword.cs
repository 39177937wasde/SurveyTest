using FinalExam.Data;
using FinalExam.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MimeKit;
//using MailKit.Net.Smtp;

namespace FinalExam.Controllers
{
    public class EditPassword : Controller
    {
        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public EditPassword(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            if (ModelState.IsValid)
            {
                // Handle your login logic here
                TempData["Message"] = "成功登入";
                return RedirectToAction("Login");
            }
            return View();
        }
        public IActionResult ForgotPassword()
        {
            if (ModelState.IsValid)
            {
                // Handle your password reset logic here
                TempData["Message"] = "驗證信已經送出";
                return RedirectToAction("ForgotPassword");
            }
            return View();
        }
        public IActionResult changePassword()
        {
            return View();
        }


        private string GenerateTemporaryPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8); // 生成8位的臨時密碼
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)//still error
        {
            if (ModelState.IsValid)
            {
                // 查找用戶
                var user = await dbManager.User.Where(u => u.UserAccount == model.Email)
                //.Select(u => new { u.UserAccount, u.Password })
                                 .FirstOrDefaultAsync();
                //foreach (var prop in user.GetType().GetProperties())
                //{
                //    var propValue = prop.GetValue(user, null);
                //    Console.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
                //    Debug.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
                //}
                //Console.WriteLine(user);
                //Debug.WriteLine(user);
                if (user != null)
                {
                    // 生成臨時密碼
                    var tempPassword = GenerateTemporaryPassword();
                    user.Password = tempPassword; // 將臨時密碼進行哈希處理後存儲
                    //user.MustChangePassword = true; // 設置用戶必須更改密碼的標誌
                    await dbManager.SaveChangesAsync();

                    // 發送臨時密碼郵件
                    SendTemporaryPasswordEmail(model.Email, tempPassword);

                    // 提示用戶臨時密碼已發送
                    TempData["Message"] = "A temporary password has been sent to your email.";
                    return RedirectToAction("Login","UserLogin");
                }
                // 如果用戶不存在，添加模型錯誤信息
                ModelState.AddModelError(string.Empty, "Email does not exist.");
            }
            return View(model);
        }



        private void SendTemporaryPasswordEmail(string email, string temporaryPassword)
        {
            //var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("管理員", "s1091785@gm.pu.edu.tw"));//寄件者
            //message.To.Add(new MailboxAddress("會員", email));//收件者
            //message.Subject = "問卷網站的暫時密碼";


            //var bodyBuilder = new BodyBuilder();
            //bodyBuilder.TextBody = $"Your temporary password is: {temporaryPassword}. Please log in and change your password immediately.";
            //bodyBuilder.HtmlBody = "<p> HTML 內容 </p>";

            //using (var smtp = new SmtpClient()) 
            //{
            //    var host = "smtp.gmail.com";
            //    int port = 465;
            //    var useSSL = true;
            //    smtp.Connect(host, port, useSSL);


            //    smtp.Send(message);
            //    smtp.Disconnect(true);
            //}

            string fromEmail = "s1091785@gm.pu.edu.tw";
            var toEmail = new MailAddress(email);
            string fromEmailPassword = "pzty ismd mktn pwjk";

            string subject = "Please confirm your email address";
            string body = $"Your verification code is: {temporaryPassword}";

            MailMessage message =new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = subject;
            message.To.Add(toEmail);
            message.Body = body;
            message.IsBodyHtml = true;



            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
                EnableSsl = true,
                UseDefaultCredentials = false
            };
            smtp.Send(message);
        }
    }
}
