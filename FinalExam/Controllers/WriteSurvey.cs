using Microsoft.AspNetCore.Mvc;

namespace FinalExam.Controllers
{
    public class WriteSurvey : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Write()
        {
            return View();
        }
    }
}
