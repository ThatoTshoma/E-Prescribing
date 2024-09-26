using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DisplayText => $"{Code} - {Description}";


    }
}
