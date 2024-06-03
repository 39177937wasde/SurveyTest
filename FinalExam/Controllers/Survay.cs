using FinalExam.Data;
using FinalExam.DTO;
using FinalExam.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using static FinalExam.Controllers.Survay;
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

        public class UserIdDto
        {
            public int UserId { get; set; }
        }


        //[Route("api/[controller]")]
        [HttpPost("create-survey")]
        public async Task<IActionResult> CreateQuestionnaire([FromBody] QuestionnaireDto questionnaireDto)
        {
            if (questionnaireDto == null)
            {
                Console.WriteLine("Received null questionnaireDto.");
                return BadRequest("Questionnaire data is null.");
            }
            if (questionnaireDto == null || string.IsNullOrEmpty(questionnaireDto.State))
            {
                return BadRequest(new { message = "State is required." });
            }

            Console.WriteLine($"Received questionnaire: {JsonConvert.SerializeObject(questionnaireDto)}");

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Int32.TryParse(userName, out var userId))
            {
                Console.WriteLine("Invalid user ID.");
                return Unauthorized("Invalid user ID.");
            }

            var user = await dbManager.User.Where(u => u.UserID == userId).SingleOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return Unauthorized("User not found.");
            }

            var questionnaire = new Questionnaire
            {
                OwnerID = userId,
                Tag = questionnaireDto.Tag,
                EndTime = questionnaireDto.EndTime,
                State = "Active",
                CreatedDate = DateTime.Now // 設置創建日期
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

        [HttpGet("get-user-questionnaires")]
        public async Task<IActionResult> GetUserQuestionnaires()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            var questionnaires = await dbManager.Questionnaires
                .Where(q => q.OwnerID == userId)
                .Select(q => new QuestionnaireDto
                {
                    QuestionnaireID = q.QuestionnaireID,
                    OwnerID = q.OwnerID,
                    Tag = q.Tag,
                    EndTime = q.EndTime,
                    Copies = q.Copies,
                    UseCopies = q.UseCopies,
                    State = q.State,
                    CreatedDate = q.CreatedDate, // 包含創建日期
                    Questions = q.Questions.Select(ques => new QuestionDto
                    {
                        QuestionID = ques.QuestionID,
                        QuestionText = ques.QuestionText,
                        QuestionType = ques.QuestionType,
                        Options = ques.Options.Select(opt => new OptionDto
                        {
                            OptionID = opt.OptionID,
                            OptionText = opt.OptionText
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(questionnaires);
        }

        [HttpGet("get-user-questionnaires/{userId}")]
        public async Task<IActionResult> GetQuestionnaire(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var userQuestionnaires = await dbManager.Questionnaires
                .Where(q => q.OwnerID == userId)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .ToListAsync();

            var result = userQuestionnaires.Select(q => new QuestionnaireDto
            {
                QuestionnaireID = q.QuestionnaireID,
                OwnerID = q.OwnerID,
                Tag = q.Tag,
                EndTime = q.EndTime,
                Copies = q.Copies,
                UseCopies = q.UseCopies,
                State = q.State,
                CreatedDate = q.CreatedDate, // 包含創建日期
                Questions = q.Questions.Select(ques => new QuestionDto
                {
                    QuestionID = ques.QuestionID,
                    QuestionText = ques.QuestionText,
                    QuestionType = ques.QuestionType,
                    Options = ques.Options.Select(opt => new OptionDto
                    {
                        OptionID = opt.OptionID,
                        OptionText = opt.OptionText
                    }).ToList()
                }).ToList()
            }).ToList();

            if (result.Count == 0)
            {
                return NotFound("No questionnaires found for the user.");
            }

            return Ok(result);
        }


        [HttpDelete("delete-questionnaire/{questionnaireId}")]
        public async Task<IActionResult> DeleteQuestionnaire(int questionnaireId)
        {
            var questionnaire = await dbManager.Questionnaires
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionnaireID == questionnaireId);

            if (questionnaire == null)
            {
                return NotFound();
            }

            dbManager.Questionnaires.Remove(questionnaire);
            await dbManager.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-question/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var question = await dbManager.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionID == questionId);

            if (question == null)
            {
                return NotFound();
            }

            dbManager.Questions.Remove(question);
            await dbManager.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-option/{optionId}")]
        public async Task<IActionResult> DeleteOption(int optionId)
        {
            var option = await dbManager.Options.FindAsync(optionId);

            if (option == null)
            {
                return NotFound();
            }

            dbManager.Options.Remove(option);
            await dbManager.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("current-user-id")]
        public IActionResult GetCurrentUserId()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName) || !int.TryParse(userName, out var userId))
            {
                return Unauthorized();
            }
            return Ok(new UserIdDto { UserId = userId });
        }



        [HttpGet("get-survey/{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            try
            {
                var survey = await dbManager.Questionnaires
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                    .Include(q => q.Owner) // 確保包含 Owner
                    .FirstOrDefaultAsync(q => q.QuestionnaireID == id);

                if (survey == null)
                {
                    return NotFound();
                }

                var surveyDto = new
                {
                    survey.QuestionnaireID,
                    survey.Tag,
                    survey.OwnerID,
                    survey.EndTime,
                    Owner = survey.Owner == null ? null : new
                    {
                        survey.Owner.UserID,
                        survey.Owner.Name
                    },
                    Questions = survey.Questions.Select(q => new
                    {
                        q.QuestionID,
                        q.QuestionText,
                        q.QuestionType,
                        Options = q.Options.Select(o => new
                        {
                            o.OptionID,
                            o.OptionText
                        }).ToList()
                    }).ToList()
                };

                return Ok(surveyDto);
            }
            catch (Exception ex)
            {
                // 打印錯誤日誌以便調試
                Console.WriteLine($"Error fetching survey with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }






        [HttpGet("available-surveys")]
        public async Task<IActionResult> GetAvailableSurveys()
        {
            var userIdResult = GetCurrentUserId();
            var userId = (int)((OkObjectResult)userIdResult).Value.GetType().GetProperty("UserId").GetValue(((OkObjectResult)userIdResult).Value, null);

            var surveys = await dbManager.Questionnaires
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .Include(s => s.Owner)
                .Where(s => s.OwnerID != userId && !dbManager.QuestionnaireRespond.Any(r => r.QuestionnaireID == s.QuestionnaireID && r.UserID == userId))
                .ToListAsync();

            var surveyDtos = surveys.Select(s => new QuestionnaireDto
            {
                QuestionnaireID = s.QuestionnaireID,
                Tag = s.Tag,
                EndTime = s.EndTime,
                CreatedDate = s.CreatedDate, // 確保傳遞 CreatedDate
                Owner = new UserDto
                {
                    UserID = s.Owner.UserID,
                    Name = s.Owner.Name
                },
                Questions = s.Questions.Select(q => new QuestionDto
                {
                    QuestionID = q.QuestionID,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        OptionID = o.OptionID,
                        OptionText = o.OptionText
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(surveyDtos);
        }













        [HttpPost("submit-response")]
        public async Task<IActionResult> SubmitResponse([FromBody] QuestionnaireResponseDto responseDto)
        {
            var userIdResult = GetCurrentUserId() as OkObjectResult;
            if (userIdResult == null)
            {
                return Unauthorized();
            }
            var userId = (userIdResult.Value as UserIdDto)?.UserId ?? 0;

            foreach (var questionResponse in responseDto.QuestionResponses)
            {
                var existingResponse = await dbManager.QuestionnaireRespond
                    .FirstOrDefaultAsync(r => r.QuestionnaireID == responseDto.QuestionnaireID &&
                                              r.QuestionID == questionResponse.QuestionID &&
                                              r.UserID == userId);

                if (existingResponse != null)
                {
                    // 更新已存在的回答
                    existingResponse.Answer = questionResponse.Answer;
                }
                else
                {
                    // 添加新的回答
                    var response = new QuestionnaireRespond
                    {
                        QuestionnaireID = responseDto.QuestionnaireID,
                        QuestionID = questionResponse.QuestionID,
                        UserID = (int)userId,
                        Answer = questionResponse.Answer
                    };

                    dbManager.QuestionnaireRespond.Add(response);
                }
            }

            await dbManager.SaveChangesAsync();

            return Ok();
        }




        [HttpPost("update-response")]
        public async Task<IActionResult> UpdateResponse([FromBody] QuestionnaireResponseDto responseDto)
        {
            var userIdResult = GetCurrentUserId() as OkObjectResult;
            if (userIdResult == null)
            {
                return Unauthorized();
            }
            var userId = (userIdResult.Value as UserIdDto)?.UserId ?? 0;

            foreach (var questionResponse in responseDto.QuestionResponses)
            {
                var existingResponse = await dbManager.QuestionnaireRespond
                    .FirstOrDefaultAsync(r => r.QuestionnaireID == responseDto.QuestionnaireID &&
                                              r.QuestionID == questionResponse.QuestionID &&
                                              r.UserID == userId);

                if (existingResponse != null)
                {
                    // 更新已存在的回答
                    existingResponse.Answer = questionResponse.Answer;
                }
                else
                {
                    // 添加新的回答
                    var response = new QuestionnaireRespond
                    {
                        QuestionnaireID = responseDto.QuestionnaireID,
                        QuestionID = questionResponse.QuestionID,
                        UserID = (int)userId,
                        Answer = questionResponse.Answer
                    };

                    dbManager.QuestionnaireRespond.Add(response);
                }
            }

            await dbManager.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("completed-surveys")]
        public async Task<IActionResult> GetCompletedSurveys()
        {
            var userIdResult = GetCurrentUserId();
            var userId = (int)((OkObjectResult)userIdResult).Value.GetType().GetProperty("UserId").GetValue(((OkObjectResult)userIdResult).Value, null);

            var completedSurveys = await dbManager.Questionnaires
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .Include(s => s.Owner)
                .Where(s => dbManager.QuestionnaireRespond.Any(r => r.QuestionnaireID == s.QuestionnaireID && r.UserID == userId))
                .ToListAsync();

            var completedSurveyDtos = completedSurveys.Select(s => new QuestionnaireDto
            {
                QuestionnaireID = s.QuestionnaireID,
                Tag = s.Tag,
                EndTime = s.EndTime,
                Owner = new UserDto
                {
                    UserID = s.Owner.UserID,
                    Name = s.Owner.Name
                },
                Questions = s.Questions.Select(q => new QuestionDto
                {
                    QuestionID = q.QuestionID,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        OptionID = o.OptionID,
                        OptionText = o.OptionText
                    }).ToList()
                }).ToList()
            }).ToList();

            Console.WriteLine(JsonConvert.SerializeObject(completedSurveyDtos, Formatting.Indented)); // Add this line to output the returned data

            return Ok(completedSurveyDtos);
        }



    }
}
