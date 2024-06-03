using FinalExam.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Controllers
{
    public class SurveyViewController : Controller
    {

        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SurveyViewController(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize]
        public IActionResult AvailableSurveys()
        {
            return View();
        }

        public IActionResult detail() 
        {
            return View();
        }
        [Authorize]
        public IActionResult completedSurveys() 
        {
            return View();
        }
        [Authorize]
        public IActionResult UserQuestionnaires()
        {
            return View();
        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            var questionnaire = dbManager.Questionnaires
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault(q => q.QuestionnaireID == id);

            if (questionnaire == null)
            {
                return NotFound();
            }

            return View(questionnaire);
        }
    }
}
