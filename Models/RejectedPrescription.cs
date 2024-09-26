using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class RejectedPrescription
    {
        public int RejectedPrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public int PrescriptionId { get; set; }
        [Display(Name = "Rejected Reason")]
        public string? RejectedReason { get; set; }
    }
}
