using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class IgnorePrescriptionReason
    {
        [Key]
        public int IgnorePrescriptionReasonId { get; set; }
        public string Reason { get; set; }
        public Prescription Prescription { get; set; }
        public int PrescriptionId { get; set; }
        public Pharmacist Pharmacist { get; set; }
        public int? PharmacistId { get; set; }
        public Surgeon Surgeon { get; set; }
        public int? SurgeonId { get;set; }
    }
}
