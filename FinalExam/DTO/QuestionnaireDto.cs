using static FinalExam.Models.Entities.Question;

namespace FinalExam.DTO
{
    public class QuestionnaireDto
    {
        //public int OwnerID { get; set; }
        //public string Tag { get; set; }
        //public DateTime EndTime { get; set; }
        //public List<QuestionDto> Questions { get; set; }

        public int QuestionnaireID { get; set; }
        public int OwnerID { get; set; }
        public string Tag { get; set; }
        public DateTime EndTime { get; set; }
        public int Copies { get; set; }
        public int UseCopies { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; } // 新增欄位
        public List<QuestionDto> Questions { get; set; }
        public UserDto Owner { get; set; }
    }
}
