using FinalExam.Data;
using FinalExam.DTO;
using FinalExam.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using static FinalExam.Models.Entities.Questionnaire;

namespace FinalExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Survay : Controller
    {
        private readonly DBManager dbManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public Survay(DBManager dbManager, IHttpContextAccessor httpContextAccessor)
        {
            this.dbManager = dbManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }



        //[Route("api/[controller]")]
        [HttpPost]
        public async Task<IActionResult> CreateQuestionnaire([FromBody] QuestionnaireDto questionnaireDto)
        {
            if (questionnaireDto == null)
            {
                return BadRequest();
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = Int32.Parse(userName);
            Debug.WriteLine(userName + " Debug");
            Console.WriteLine(userName + " Console");
            var user = await dbManager.User.Where(u => u.UserID == userId).SingleOrDefaultAsync();
            //foreach (var prop in user.GetType().GetProperties())
            //{
            //    var propValue = prop.GetValue(user, null);
            //    Console.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //    Debug.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
            //}

            if (user == null)
            {
                return Unauthorized();
            }
            // 創建新的問卷
            var questionnaire = new Questionnaire
            {
                OwnerID = userId,
                Tag = questionnaireDto.Tag,
                EndTime = questionnaireDto.EndTime,
                State = "Active" // 假設問卷的初始狀態是 Active
            };

            dbManager.Questionnaires.Add(questionnaire);
            await dbManager.SaveChangesAsync();

            foreach (var questionDto in questionnaireDto.Questions)
            {
                var question = new Question
                {
                    QuestionnaireID = questionnaire.QuestionnaireID,
                    QuestionText = questionDto.QuestionText,
                    QuestionType = questionDto.QuestionType
                };

                dbManager.Questions.Add(question);
                await dbManager.SaveChangesAsync();

                foreach (var optionDto in questionDto.Options)
                {
                    var option = new Option
                    {
                        QuestionID = question.QuestionID,
                        OptionText = optionDto.OptionText
                    };

                    dbManager.Options.Add(option);
                }
            }

            await dbManager.SaveChangesAsync();
            return Ok();
        }


        public IActionResult UserQuestionnaires()
        {
            //GetUserQuestionnaires();
            return View();
        }

        //[HttpGet("get-user-questionnaires")]
        //public async Task<IActionResult> GetUserQuestionnaires()
        //{
        //    //var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
        //    var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var userId = Int64.Parse(userName);


        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        return BadRequest("User ID not found.");
        //    }

        //    // 尝试解析 userIdString
        //    if (!int.TryParse(userName, out var userid))
        //    {
        //        return BadRequest("Invalid User ID.");
        //    }
        //    //Debug.WriteLine(userName);
        //    //Console.WriteLine(userName);
        //    //var user = await dbManager.User.Where(u => u.UserID == userId).SingleOrDefaultAsync();
        //    //foreach (var prop in user.GetType().GetProperties())
        //    //{
        //    //    var propValue = prop.GetValue(user, null);
        //    //    Console.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
        //    //    Debug.WriteLine($"{prop.Name}: {(propValue ?? "NULL")}");
        //    //}
        //    //if (user == null)
        //    //{
        //    //    return NotFound();
        //    //}//剛剛到這邊 Question有問題

        //    //var questionnaires = await dbManager.Questionnaires
        //    //    .Where(q => q.OwnerID == userId)
        //    //    .ToListAsync();

        //    //foreach (var questionnaire in questionnaires)
        //    //{
        //    //    Console.WriteLine($"ID: {questionnaire.QuestionnaireID}, Tag: {questionnaire.Tag}, EndTime: {questionnaire.EndTime},+++++++++++++++++++");
        //    //    Debug.WriteLine($"ID: {questionnaire.QuestionnaireID}, Tag: {questionnaire.Tag}, EndTime: {questionnaire.EndTime},+++++++++++++++++++++");
        //    //}

        //    // 查询问卷
        //    var questionnaires = await dbManager.Questionnaires
        //.Where(q => q.OwnerID == userId)
        //.Include(q => q.Questions)
        //.ThenInclude(q => q.Options)
        //.ToListAsync();

        //    // 使用匿名对象避免循环引用
        //    var result = questionnaires.Select(q => new
        //    {
        //        q.QuestionnaireID,
        //        q.OwnerID,
        //        q.Tag,
        //        q.EndTime,
        //        q.Copies,
        //        q.UseCopies,
        //        q.State,
        //        Questions = q.Questions.Select(ques => new
        //        {
        //            ques.QuestionID,
        //            ques.QuestionText,
        //            ques.QuestionType,
        //            Options = ques.Options.Select(opt => new
        //            {
        //                opt.OptionID,
        //                opt.OptionText
        //            })
        //        })
        //    }).ToArray();

        //    foreach (var questionnaire in questionnaires)
        //    {
        //        Console.WriteLine($"ID: {questionnaire.QuestionnaireID}, Tag: {questionnaire.Tag}, EndTime: {questionnaire.EndTime},+++++++++++++++++++");
        //        Debug.WriteLine($"ID: {questionnaire.QuestionnaireID}, Tag: {questionnaire.Tag}, EndTime: {questionnaire.EndTime},+++++++++++++++++++++");
        //    }


        //    return Ok(result);

        //}
        [HttpGet("get-questionnaire/{id}")]
        public async Task<IActionResult> GetQuestionnaire(int id)
        {
            var questionnaire = await dbManager.Questionnaires
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionnaireID == id);

            if (questionnaire == null)
            {
                return NotFound();
            }

            var result = new
            {
                questionnaire.QuestionnaireID,
                questionnaire.OwnerID,
                questionnaire.Tag,
                questionnaire.EndTime,
                questionnaire.Copies,
                questionnaire.UseCopies,
                questionnaire.State,
                Questions = questionnaire.Questions.Select(ques => new
                {
                    ques.QuestionID,
                    ques.QuestionText,
                    ques.QuestionType,
                    Options = ques.Options.Select(opt => new
                    {
                        opt.OptionID,
                        opt.OptionText
                    })
                })
            };

            return Ok(result);
        }
    }
}
