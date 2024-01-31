using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int id { get; set; }
        [Required]
        public string? EMail { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
