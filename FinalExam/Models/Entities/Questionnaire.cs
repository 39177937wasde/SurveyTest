using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinalExam.Models.Entities
{
    public class Questionnaire
    {
        [Key]
        public int QuestionnaireID { get; set; }
        public int OwnerID { get; set; }
        public string Tag { get; set; }
        public DateTime EndTime { get; set; }
        public int Copies { get; set; }
        public int UseCopies { get; set; }
        public string State { get; set; }
        [ForeignKey("OwnerID")]
        public User Owner { get; set; }
        public DateTime CreatedDate { get; set; } // 新增欄位
        [JsonIgnore]
        public ICollection<Question> Questions { get; set; }
    }
}
