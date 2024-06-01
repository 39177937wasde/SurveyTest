using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalExam.Models.Entities
{
    public class Option
    {
        [Key]
        public int OptionID { get; set; }
        public int QuestionID { get; set; }
        public string OptionText { get; set; }
        [ForeignKey("QuestionID")]
        public Question Question { get; set; }
    }
}
