using static FinalExam.Models.Entities.Question;

namespace FinalExam.DTO
{
    public class QuestionnaireDto
    {
        public int OwnerID { get; set; }
        public string Tag { get; set; }
        public DateTime EndTime { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
