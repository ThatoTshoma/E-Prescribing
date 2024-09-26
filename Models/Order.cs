using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Is Urgent?")]
        public string IsUrgent { get; set; }
        public string Status { get; set; }
        public string? IgnoreReason { get; set; }
        public Anaesthesiologist Anaesthesiologist { get; set; }
        [Display(Name = "Anaesthesiologist")]
        public int AnaesthesiologistId { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient")]
        public int? PatientId { get; set; }
        public Pharmacist Pharmacist { get; set; }
        [Display(Name = "Pharmacist")]
        public int? PharmacistId { get; set; }
        public List<MedicationOrder> MedicationOrders { get; set; }
        public List<PrescribedMedication> PrescribedMedications { get; set; }



    }
}
