using FinalExam.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalExam.Views.Shared.Components
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserNameViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            string username = user.Identity.IsAuthenticated ? user.FindFirst(ClaimTypes.Name)?.Value : "未登入";
            return View("Default", username);
        }
    }
}
