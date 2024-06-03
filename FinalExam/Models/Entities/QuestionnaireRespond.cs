using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.Entities
{
    public class QuestionnaireRespond
    {
        [Key]
        public int RespondID { get; set; }
        public int QuestionnaireID { get; set; }
        public int QuestionID { get; set; }
        public int UserID { get; set; }
        public string Answer { get; set; }
        public Questionnaire Questionnaire { get; set; }
    }
}
