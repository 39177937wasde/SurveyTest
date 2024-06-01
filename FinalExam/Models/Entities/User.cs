using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinalExam.Models.Entities
{
    public class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        
        public string UserAccount { get; set; }

       
        public string Password { get; set; }

        
        public string Name { get; set; }

        public string? Avatar { get; set; } = null;

        public int Points { get; set; } = 0;
        [JsonIgnore]
        public ICollection<Questionnaire> Questionnaires { get; set; }
    }
}
