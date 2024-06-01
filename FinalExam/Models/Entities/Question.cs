using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinalExam.Models.Entities
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }
        public int QuestionnaireID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        [ForeignKey("QuestionnaireID")]
        public Questionnaire Questionnaire { get; set; }
        [JsonIgnore]
        public ICollection<Option> Options { get; set; }
        //public Questionnaire Questionnaire { get; set; }
    }
}
