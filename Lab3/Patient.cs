using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3
{
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 1)]
        public int Id { get; set; }
        [Required] public string? FullName { get; set; }
        [Required] public int Age { get; set; }
        [Required] public string? Gender { get; set; }
        [Required] public string? PlaceOfResidence { get; set; }
        [Required] public string? DiagnosisCode { get; set; }
        [Required] public string? DiagnosisName { get; set; }
        [Required] public int Days { get; set; }
        [Required] public double Cost { get; set; }
        [Required] public int? TariffId { get; set; }
        public Tariff? Tariff { get; set; }
    }
}