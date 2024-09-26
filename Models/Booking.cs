using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime Date { get; set; }
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public Surgeon Surgeon { get; set; }
        [Display(Name = "Surgeon Name")]
        public int SurgeonId { get;set; }
        public Anaesthesiologist Anaesthesiologist { get; set; }
        [Display(Name = "Anaesthesiologist")]
        public int AnaesthesiologistId { get; set; }
        public Theatre Theatre { get; set; }
        [Display(Name = "Theatre")]
        public int TheatreId { get; set; }

        public bool Status { get; set; }
        public string Session {  get; set; }
        public ICollection<PatientTreatment> PatientTreatments { get; set; }

    }
}
