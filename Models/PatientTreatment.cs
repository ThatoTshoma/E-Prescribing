using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PatientTreatment
    {
        [Key]
        public int PatientTreatmentId { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set; }
        public Treatment Treatment { get; set; }
        [Display(Name = "Treatment")]
        public int TreatmentId { get; set; }
        public Booking Booking { get; set; }
        [Display(Name = "Booking")]
        public int BookingId { get; set; }
        [NotMapped]
        public List<int> SelectedTreaments { get; set; }
    }
}
