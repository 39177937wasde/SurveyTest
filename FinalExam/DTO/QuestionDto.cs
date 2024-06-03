using static FinalExam.Models.Entities.Question;

namespace FinalExam.DTO
{
    public class QuestionDto
    {
        //public string QuestionText { get; set; }
        //public string QuestionType { get; set; }
        //public List<OptionDto> Options { get; set; }
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public List<OptionDto> Options { get; set; }
    }
}
