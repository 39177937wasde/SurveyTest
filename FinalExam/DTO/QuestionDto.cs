using static FinalExam.Models.Entities.Question;

namespace FinalExam.DTO
{
    public class QuestionDto
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public List<OptionDto> Options { get; set; }
    }
}
